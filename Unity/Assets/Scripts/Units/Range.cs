using UnityEngine;
using System.Collections.Generic;

/**
  * @class ActionField
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
[System.Serializable]
public class Range {
    
    public enum FIELD
    {
        NONE,
        CIRCLE,
        FORWARD,
        GROUP
    }

    public FIELD field;
    public int angle;
    public int distance;

    public static void ShowCircle(Vector3 position, Vector3 forward, Vector3 right, float angle, float distance)
    {
        float a2 = angle * Mathf.Deg2Rad / 2.0f;
        float d = distance * Game.settings.distanceRatio * Mathf.Tan(a2);

        Vector3 first = (forward * distance * Game.settings.distanceRatio) + (right * d);
        if (a2 > (Mathf.PI / 2.0f))
        {
            first.x = -first.x;
            first.z = -first.z;
        }
        if (a2 == Mathf.PI / 2.0f)
        {
            first = right;
        }

        first.Normalize();
        first *= distance * Game.settings.distanceRatio;

        Vector3 last = (forward * distance * Game.settings.distanceRatio) - (right * d);
        if (a2 > (Mathf.PI / 2.0f))
        {
            last.x = -last.x;
            last.z = -last.z;
        }
        if (a2 == Mathf.PI / 2.0f)
        {
            last = -right;
        }

        last.Normalize();
        last *= distance * Game.settings.distanceRatio;

        if (angle < 360)
        {
            Gizmos.DrawLine(position, position + first);
            Gizmos.DrawLine(position, position + last);
        }

        Vector3 previousA = position + (forward * distance * Game.settings.distanceRatio);
        Vector3 previousB = position + (forward * distance * Game.settings.distanceRatio);

        for (float a = 0; a <= a2; a += 0.1f)
        {
            d = distance * Game.settings.distanceRatio * Mathf.Tan(a);

            Vector3 pos = (forward * distance * Game.settings.distanceRatio) + (right * d);
            if (a > (Mathf.PI / 2.0f))
            {
                pos.x = -pos.x;
                pos.z = -pos.z;
            }
            if (a == Mathf.PI / 2.0f)
            {
                last = right;
            }

            pos.Normalize();
            pos *= distance * Game.settings.distanceRatio;

            Gizmos.DrawLine(previousA, position + pos);
            previousA = position + pos;

            pos = (forward * distance * Game.settings.distanceRatio) - (right * d);
            if (a > (Mathf.PI / 2.0f))
            {
                pos.x = -pos.x;
                pos.z = -pos.z;
            }

            pos.Normalize();
            pos *= distance * Game.settings.distanceRatio;

            Gizmos.DrawLine(previousB, position + pos);
            previousB = position + pos;
        }

        Gizmos.DrawLine(previousA, position + first);
        Gizmos.DrawLine(previousB, position + last);
    }
    
    public static void OnDrawGizmos(Unit u)
    {
        if (Game.started)
        {
            Color oldC = Gizmos.color;
            
            Gizmos.color = Color.blue;
            switch (u.range.field)
            {
                case FIELD.CIRCLE:
                    ShowCircle(u.transform.position, u.transform.forward, u.transform.right, u.range.angle, u.range.distance);
                    break;
                case FIELD.FORWARD:
                    Gizmos.DrawLine(u.transform.position, u.transform.position + (u.transform.forward * u.range.distance * Game.settings.distanceRatio));
                    break;

                case FIELD.GROUP:
                    Rect groupRect = u.group.GetRect(5);
                    RectUtility.GizmosRect(groupRect);
                    break;

                default: // FIELD.NONE
                    break;
            }

            Gizmos.color = oldC;
        }
    }

    public static bool IsInRange(Unit u, Unit target)
    {
        switch (u.range.field)
        {
            case FIELD.CIRCLE:
                IsInCircle(u, target);
                break;

            case FIELD.FORWARD:
                IsForward(u, target);
                break;

            case FIELD.GROUP:
                return u.group.Contains(target);
        }
        
        return false;
    }

    public static bool IsInCircle(Unit u, Unit target)
    {
        // First step : field.
        float d = Vector3.Distance(u.transform.position, target.transform.position);
        if (d > u.range.distance)
        {
            return false;
        }

        // Full circle : no need to compute anything
        if (u.range.angle >= 360)
        {
            return true;
        }

        // Just in case..
        if (u.range.angle <= 0)
        {
            Debug.LogWarning("You should set the angle !");
            return false;
        }

        // Second step : angle
        Vector3 unitToTarget = target.transform.position - u.transform.position;
        Vector3 forward = u.transform.forward;

        float angle = Vector3.Angle(forward, unitToTarget);
        return (angle <= u.range.angle / 2);
    }

    public static bool IsForward(Unit u, Unit target)
    {
        return false;
    }
}
