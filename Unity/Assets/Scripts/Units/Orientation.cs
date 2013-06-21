using Vector3 = UnityEngine.Vector3;

//public enum OrientationAbsolute 
public enum Orientation
{    
    NORTH,
    EAST,
    WEST,
    SOUTH
}

//public enum OrientationRelative
//{
//    FORWARD,
//    RIGHT,
//    LEFT,
//    BEHIND
//}

public static class OrientationUtility
{
    //public static Vector3 ToVector3(Vector3 forward, OrientationRelative o)
    //{      
    //    switch (o)
    //    {
    //        case OrientationRelative.FORWARD:
    //            break;
    //        case OrientationRelative.BEHIND:
    //            forward.z *= -1;
    //            break;
    //        case OrientationRelative.LEFT:
    //            forward = Vector3.Cross(Vector3.up, forward);
    //            break;
    //        case OrientationRelative.RIGHT:
    //            forward = Vector3.Cross(Vector3.up, forward);
    //            forward.x *= -1;
    //            break;
    //    }
    //
    //    return forward;
    //}
    //
    //public static Vector3 ToVector3(OrientationAbsolute o)
    //{
    //    switch (o)
    //    {
    //        case OrientationAbsolute.NORTH:
    //            return Vector3.forward;
    //        case OrientationAbsolute.SOUTH:
    //            return -Vector3.forward;
    //        case OrientationAbsolute.EAST:
    //            return Vector3.right;
    //        case OrientationAbsolute.WEST:
    //            return -Vector3.right;
    //    }
    //
    //    return Vector3.zero;
    //}

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