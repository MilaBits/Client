using System;

[Serializable]
public struct AdjacentData
{
    public Directions directions;
    public Connectable NorthWest;
    public Connectable North;
    public Connectable NorthEast;
    public Connectable East;
    public Connectable SouthEast;
    public Connectable South;
    public Connectable SouthWest;
    public Connectable West;

}