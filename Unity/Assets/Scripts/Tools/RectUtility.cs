using UnityEngine;
using System.Collections.Generic;

/**
  * @class RectUtility
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public static class RectUtility {

    public static Rect MinMaxRect(Vector3 a, Vector3 b)
    {
        Rect result = new Rect(
            Mathf.Min(a.x, b.x),
            Mathf.Min(a.z, b.z),
            Mathf.Abs(a.x - b.x),
            Mathf.Abs(a.z - b.z)
        );

        return result;
    }

    public static void DebugRect(Rect r)
    {
        Vector3 a, b, c, d;

        a = new Vector3(r.xMin, 0, r.yMin);
        b = new Vector3(r.xMax, 0, r.yMin);
        c = new Vector3(r.xMax, 0, r.yMax);
        d = new Vector3(r.xMin, 0, r.yMax);

        Debug.DrawLine(a, b);
        Debug.DrawLine(b, c);
        Debug.DrawLine(c, d);
        Debug.DrawLine(d, a);
    }

    public static void GizmosRect(Rect r)
    {
        Vector3 a, b, c, d;

        a = new Vector3(r.xMin, 0, r.yMin);
        b = new Vector3(r.xMax, 0, r.yMin);
        c = new Vector3(r.xMax, 0, r.yMax);
        d = new Vector3(r.xMin, 0, r.yMax);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }

    public static bool ContainsXZ(Rect r, Vector3 p)
    {
        p.y = p.z;
        return r.Contains(p);
    }
}
