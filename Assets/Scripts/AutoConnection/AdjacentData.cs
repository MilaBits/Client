using System;

[Serializable]
public struct AdjacentData
{
    public Directions Connections;
    public Connectable North;
    public Connectable East;
    public Connectable South;
    public Connectable West;

    public Connectable NorthEast;
    public Connectable SouthEast;
    public Connectable SouthWest;
    public Connectable NorthWest;
}