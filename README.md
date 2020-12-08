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
