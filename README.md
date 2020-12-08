# WktParser

A C# Parser for Well-Known Text(WKT) representation of geometry.

In 2011, Parker Martin had geographic data represented as
[Well-Known Text](https://en.wikipedia.org/wiki/Well-known_text_representation_of_geometry)
and requested the ability to see them in ArcGIS.

This project parses Well-Known Text into an internal C# data model.
I intended to use the ArcObjects SDK to create shapes that
could be drawn as graphics or saved to a feature class. However
Parker found an alternative solution before this project was
finished, so it was put on indefinite hold.


## Build

Open the solution in MS Visual Studio (originally developed with 2010
and last tested with 2019 Community edition).  Select Debug or Release
from the drop down on the main toolbar and click the start button on
the main toolbar.  This will run a console window with the output of
the testdata string that is uncommented in the main() method.

## Deploy

This is a command line application and the deployable exe can be found
in the bin/debug or bin/release folder.  the exe can be copied to and
run from any convenient location.

## Using

This app just prints the fixed sample data from the main() method,
so it has no practicle use case at this time.
