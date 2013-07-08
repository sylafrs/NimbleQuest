using Vector3 = UnityEngine.Vector3;

public enum Orientation
{    
    NORTH,
    EAST,
    WEST,
    SOUTH
}

public static class OrientationUtility
{
    public static Orientation TurnLeft(Orientation o)
    {
        switch (o)
        {
            case Orientation.NORTH:
                return Orientation.WEST;
            case Orientation.WEST:
                return Orientation.SOUTH;
            case Orientation.SOUTH:
                return Orientation.EAST;
            case Orientation.EAST:
                return Orientation.NORTH;
        }

        return Orientation.NORTH;
    }

    public static Orientation TurnRight(Orientation o)
    {
        switch (o)
        {
            case Orientation.NORTH:
                return Orientation.EAST;
            case Orientation.WEST:
                return Orientation.NORTH;
            case Orientation.SOUTH:
                return Orientation.WEST;
            case Orientation.EAST:
                return Orientation.SOUTH;
        }

        return Orientation.NORTH;
    }

    public static Vector3 ToVector3(Orientation o)
    {
        switch (o)
        {
            case Orientation.NORTH:
                return Vector3.forward;
            case Orientation.SOUTH:
                return -Vector3.forward;
            case Orientation.EAST:
                return Vector3.right;
            case Orientation.WEST:
                return -Vector3.right;
        }
    
        return Vector3.zero;
    }

    public static Orientation Inverse(Orientation o)
    {
        switch (o)
        {
            case Orientation.NORTH:
                return Orientation.SOUTH;
            case Orientation.SOUTH:
                return Orientation.NORTH;
            case Orientation.EAST:
                return Orientation.WEST;
            case Orientation.WEST:
                return Orientation.EAST;
        }

        return Orientation.EAST;
    }
}