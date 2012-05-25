using System;
using System.Collections.Generic;
using System.Linq;

namespace WktParser
{
	class MainClass
	{
        public static void Main(string[] args)
        {
            //string[] testdata = { "Point z (12.3 3 4)", "Point (12.3 3)", "Point m  (2.3 3 4)", "Point zm (12.3 3 4.1 5.5)" };
            //string[] testdata = { "Point z ", " Point   (12.3 3)", " pOint M  (2.3 3 4)", "Point mz (12.3 3)" };
            //string[] testdata = { "multiPoint z ((1 2 3),(2 3 4))", "multiPoint ((1 2), (3 4), (5 6.1))", "multiPoint ((1 2))" };
            //string[] testdata = { "Linestring z   (1 2 3,2 3 4,4 5 6,  7   8.1  9  )", "linestring (1.1 2.2, 3.3 4.4, 5.5 6.6)" };
            //string[] testdata = { "multilinestring ((1.1 2.2, 3.3 4.4, 5.5 6.6), (1.1 2.2, 3.3 4.4, 5.5 6.6))", "multilinestring ((1.1 2.2, 3.3 4.4, 5.5 6.6))" };
            //string[] testdata = { "polygon ((1.1 2.2, 3.3 4.4, 5.5 6.6), (1.1 2.2, 3.3 4.4, 5.5 6.6))", "polygon ((1.1 2.2, 3.3 4.4, 5.5 6.6))" };
            //string[] testdata = { "multipolygon (((1.1 2.2, 3.3 4.4, 5.5 6.6), (1.1 2.2, 3.3 4.4, 5.5 6.6)), ((1.1 2.2, 3.3 4.4, 5.5 6.6)))" };
            //string[] testdata = { "Point z (12.3 3 4)", "Point (12.3 3)", "Point m  (2.3 3 4)", "Point zm (12.3 3 4.1 5.5)" };
            string[] testdata = { "geometrycollection  (point (12.3 3), linestring (1.1 2.2, 3.3 4.4, 5.5 6.6), polygon ((1.1 2.2, 3.3 4.4, 5.5 6.6)))" };
            foreach (var s in testdata)
            {
                Console.WriteLine(s + " => " + ConvertWkt(s));
            }
            Console.ReadLine();
        }

        private static string ConvertWkt(string s)
        {
            var wkt = new WktText(s);
            string result="";

            if (wkt.Type == WktType.GeometryCollection)
            {
                result = string.Format("{0} {1} (", wkt.Type, wkt.ZmText);
                foreach (var geometry in wkt.Token.Tokens)
                {
                    string text = geometry.Text.Substring(geometry.StartIndex,
                                                          1 + geometry.EndIndex - geometry.StartIndex);
                    Console.WriteLine(text);
                    //throw an error if geometry zm != geometry collection zm
                    result += ConvertWkt(text) + ", ";
                }
                result += ")";
            }
            //if (wkt.Type == WktType.Tin)
            //if (wkt.Type == WktType.PolyhedralSurface)
            if (wkt.Type == WktType.MultiPolygon)
                {
                result = string.Format("{0} {1} (", wkt.Type, wkt.ZmText);
                foreach (var polygon in wkt.Token.Tokens)
				{
                    result += "(";
                    foreach (var ring in polygon.Tokens)
					{
                        result += "(";
                        foreach (var point in ring.Tokens)
						{
                            result += FixPoint(point.Coords, wkt.CoordinateCount) + ", ";
                        }
                        result += "), ";
                    }
                    result += "), ";
                }
                result += ")";
            }


            //if (wkt.Type == WktType.MultiLineString)
            //if (wkt.Type == WktType.Triangle)
            if (wkt.Type == WktType.Polygon)
            {
                result = string.Format("{0} {1} (", wkt.Type, wkt.ZmText);
                foreach (var ring in wkt.Token.Tokens)
                {
                    result += "(";
                    foreach (var point in ring.Tokens)
                    {
                        result += FixPoint(point.Coords, wkt.CoordinateCount) + ", ";
                    }
                    result += "), ";
                }
                result += ")";
            }

            if (wkt.Type == WktType.LineString)
            {
                result = string.Format("{0} {1} (", wkt.Type, wkt.ZmText);
                foreach (var point in wkt.Token.Tokens)
                {
                    result += FixPoint(point.Coords, wkt.CoordinateCount) + ", ";
                }
                result += ")";
            }

            if (wkt.Type == WktType.MultiLineString)
            {
                result = string.Format("{0} {1} (", wkt.Type, wkt.ZmText);
                foreach (var path in wkt.Token.Tokens)
                {
                    result += "(";
                    foreach (var point in path.Tokens)
                    {
                        result += FixPoint(point.Coords, wkt.CoordinateCount) + ", ";
                    }
                    result += "), ";
                }
                result += ")";
            }
			
			if (wkt.Type == WktType.MultiPoint)
			{
                result = string.Format("{0} {1} (", wkt.Type, wkt.ZmText);
			    result = wkt.Token.Tokens
                    .Select(point => FixPoint(point.Coords, wkt.CoordinateCount))
                    .Aggregate(result, (current, coordtext) => current + "(" + coordtext + "), ");
			    result = result.Substring(0,result.Length-2) + ")";
			}
			
			if (wkt.Type == WktType.Point)
			{
                var coordtext = FixPoint(wkt.Token.Coords, wkt.CoordinateCount);
                result = string.Format("{0} {1} ({2})", wkt.Type, wkt.ZmText, coordtext);
			}

            return result;
        }

        private static string FixPoint(IEnumerable<double> point, int coordCount)
        {
            var coords = point.Select(n => 2.0 * n).ToArray();

            if (coords.Length == 0)
                //empty point is valid
                return "";
            if (coords.Length != coordCount)
                throw new ArgumentException("Wrong number of coordinates");

            return coords.Aggregate("", (a, n) => a == "" ? a + n : a + " " + n);
        }

	}
		         
	public class WktText
	{
		public WktText(string s)
		{
			if (string.IsNullOrEmpty(s))
				throw new ArgumentException("WKT is empty");
			ParsePrefix(s);
		}
		
		public bool HasZ { get; private set; }
		public bool HasM { get; private set; }
		public int CoordinateCount { get; private set; }
		public WktType Type { get; private set; }
        public WktToken Token { get; private set; }

		public void ParsePrefix(string s)
		{
			int startIndex = s.IndexOf('(');
		    int endIndex = s.LastIndexOf(')');
		    string prefix = startIndex == -1 ? s : s.Substring(0,startIndex);

		    HasZ = false;
		    HasM = false;
		    CoordinateCount = 2;
            prefix = prefix.Trim().ToLower();
            if (prefix.EndsWith(" z"))
            {
                HasZ = true;
                CoordinateCount = 3;
            }
            if (prefix.EndsWith(" m"))
            {
                HasM = true;
                CoordinateCount = 3;
            }
		    if (prefix.EndsWith(" zm"))
            {
                HasM = true;
                HasZ = true;
                CoordinateCount = 4;
            }

            Type = WktType.None;
            if (prefix.StartsWith("point"))
                Type = WktType.Point;
            if (prefix.StartsWith("linestring"))
                Type = WktType.LineString;
            if (prefix.StartsWith("polygon"))
                Type = WktType.Polygon;
            if (prefix.StartsWith("polyhedralsurface"))
                Type = WktType.PolyhedralSurface;
            if (prefix.StartsWith("triangle"))
                Type = WktType.Triangle;
            if (prefix.StartsWith("tin"))
                Type = WktType.Tin;
            if (prefix.StartsWith("multipoint"))
                Type = WktType.MultiPoint;
            if (prefix.StartsWith("multilinestring"))
                Type = WktType.MultiLineString;
            if (prefix.StartsWith("multipolygon"))
                Type = WktType.MultiPolygon;
            if (prefix.StartsWith("geometrycollection"))
                Type = WktType.GeometryCollection;

            Token = new WktToken(s,startIndex, endIndex);
        }

        public string ZmText
        {
            get { return (HasZ ? "Z" : "") + (HasM ? "M" : ""); }
        }
	}
	
	public class WktToken
	{
		public WktToken(string s, int startIndex, int endIndex)
		{
		    Text = s;
		    StartIndex = startIndex;
		    EndIndex = endIndex;
            //remove optional whitespace and/or parens at ends of token
		    if (IsEmpty)
                return;
            while (Char.IsWhiteSpace(Text[StartIndex]))
                StartIndex++;
		    bool removedLeadingParen = false;
            if (Text[StartIndex] == '(')
            {
                StartIndex++;
                removedLeadingParen = true;
            }
            while (Char.IsWhiteSpace(Text[EndIndex]))
                EndIndex--;
            if (Text[EndIndex] == ')' && removedLeadingParen)
                EndIndex--;
        }

        public string Text { get; private set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }

	    public bool IsEmpty
	    {
	        get { return StartIndex < 0 || EndIndex < StartIndex; }
	    }

	    public IEnumerable<WktToken> Tokens
        {
            get
            {
                if (IsEmpty)
                    yield break;

                int currentStart = StartIndex;
                //currentStart may be a '(', do not let currentEnd go past it without nesting.
                int currentEnd = StartIndex;
                int nesting = 0;

                while (true)
                {
                    if (currentEnd >= EndIndex)
                    {
                        yield return new WktToken(Text, currentStart, EndIndex);
                        yield break;
                    }

                    if (Text[currentEnd] == '(')
                        nesting++;
                    if (Text[currentEnd] == ')')
                        nesting--;

                    if (nesting == 0 && Text[currentEnd] == ',')
                    {
                        yield return new WktToken(Text, currentStart, currentEnd-1);
                        currentStart = currentEnd + 1;
                        while (currentStart < EndIndex && Char.IsWhiteSpace(Text[currentStart]))
                            currentStart++;
                        //currentStart may be a '(', do not let currentEnd go past it without nesting.
                        currentEnd = currentStart-1;
                    }
                    currentEnd++;
                }
            }
        }


		public IEnumerable<double> Coords
		{
			get
			{
                if (IsEmpty)
                    return new double[0];
			    string text = Text.Substring(StartIndex, 1 + EndIndex - StartIndex);
			    string[] words = text.Split((Char[])null, StringSplitOptions.RemoveEmptyEntries);
			    return words.Select(Convert.ToDouble);
			}
		}
	}
	
	public enum WktType
	{
		None,
        Point,
        LineString,
        Polygon,
        Triangle,
        PolyhedralSurface,
        Tin,
        MultiPoint,
        MultiLineString,
        MultiPolygon,
        GeometryCollection,
	}
}
