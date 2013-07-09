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
    
    public void OnDrawGizmos(Unit u)
    {
        if (Game.started)
        {
            Color oldC = Gizmos.color;

            switch (field)
            {
                case FIELD.CIRCLE:

                    Gizmos.color = Color.blue;

                    float a2 = angle * Mathf.Deg2Rad / 2.0f;                    
                    float d = distance * Game.settings.distanceRatio * Mathf.Tan(a2);

                    Vector3 first = (u.transform.forward * distance * Game.settings.distanceRatio) + (u.transform.right * d);
                    if (a2 > (Mathf.PI / 2.0f))
                    {
                        first.x = -first.x;
                        first.z = -first.z;
                    }
                    if (a2 == Mathf.PI / 2.0f)
                    {
                        first = u.transform.right;
                    }

                    first.Normalize();
                    first *= distance * Game.settings.distanceRatio;
                    
                    Vector3 last = (u.transform.forward * distance * Game.settings.distanceRatio) - (u.transform.right * d);
                    if (a2 > (Mathf.PI / 2.0f))
                    {
                        last.x = -last.x;
                        last.z = -last.z;
                    }
                    if (a2 == Mathf.PI / 2.0f)
                    {
                        last = -u.transform.right;
                    }

                    last.Normalize();
                    last *= distance * Game.settings.distanceRatio;

                    if (angle < 360)
                    {
                        Gizmos.DrawLine(u.transform.position, u.transform.position + first);
                        Gizmos.DrawLine(u.transform.position, u.transform.position + last);
                    }

                    Vector3 previousA = u.transform.position + (u.transform.forward * distance * Game.settings.distanceRatio);
                    Vector3 previousB = u.transform.position + (u.transform.forward * distance * Game.settings.distanceRatio);

                    for (float a = 0; a <= a2; a += 0.1f)
                    {                        
                        d = distance * Game.settings.distanceRatio * Mathf.Tan(a);

                        Vector3 pos = (u.transform.forward * distance * Game.settings.distanceRatio) + (u.transform.right * d);
                        if (a > (Mathf.PI / 2.0f))
                        {
                            pos.x = -pos.x;
                            pos.z = -pos.z;
                        }
                        if (a == Mathf.PI / 2.0f)
                        {
                            last = u.transform.right;
                        }

                        pos.Normalize();
                        pos *= distance * Game.settings.distanceRatio;

                        Gizmos.DrawLine(previousA, u.transform.position + pos);
                        previousA = u.transform.position + pos;

                        pos = (u.transform.forward * distance * Game.settings.distanceRatio) - (u.transform.right * d);
                        if (a > (Mathf.PI / 2.0f))
                        {
                            pos.x = -pos.x;
                            pos.z = -pos.z;
                        }

                        pos.Normalize();
                        pos *= distance * Game.settings.distanceRatio;

                        Gizmos.DrawLine(previousB, u.transform.position + pos);
                        previousB = u.transform.position + pos;
                    }

                    Gizmos.DrawLine(previousA, u.transform.position + first);
                    Gizmos.DrawLine(previousB, u.transform.position + last);

                    break;
                case FIELD.FORWARD:
                    break;
                case FIELD.GROUP:
                    break;
                default: // FIELD.NONE
                    break;
            }

            Gizmos.color = oldC;
        }
    }
}
