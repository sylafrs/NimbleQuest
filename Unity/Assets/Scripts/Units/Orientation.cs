using Vector3 = UnityEngine.Vector3;

public enum OrientationAbsolute 
{    
    NORTH,
    EAST,
    WEST,
    SOUTH
}

public enum OrientationRelative
{
    FORWARD,
    RIGHT,
    LEFT,
    BEHIND
}

public static class OrientationUtility
{
    public static Vector3 ToVector3(Vector3 forward, OrientationRelative o)
    {      
        switch (o)
        {
            case OrientationRelative.FORWARD:
                break;
            case OrientationRelative.BEHIND:
                forward.z *= -1;
                break;
            case OrientationRelative.LEFT:
                forward = Vector3.Cross(Vector3.up, forward);
                break;
            case OrientationRelative.RIGHT:
                forward = Vector3.Cross(Vector3.up, forward);
                forward.x *= -1;
                break;
        }

        return forward;
    }

    public static Vector3 ToVector3(OrientationAbsolute o)
    {
        switch (o)
        {
            case OrientationAbsolute.NORTH:
                return Vector3.forward;
            case OrientationAbsolute.SOUTH:
                return -Vector3.forward;
            case OrientationAbsolute.EAST:
                return Vector3.right;
            case OrientationAbsolute.WEST:
                return -Vector3.right;
        }

        return Vector3.zero;
    }
}