using System;

[Flags]
public enum Directions
 {
     None = 0,
     North = 1,
     East = 2,
     South = 4,
     West = 8,
     NorthEast = 16,
     SouthEast = 32,
     SouthWest = 64,
     NorthWest = 128
 }