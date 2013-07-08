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
        Color oldC = Gizmos.color;

        switch (field)
        {
            case FIELD.CIRCLE:
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(u.transform.position, distance);
                Gizmos.color = Color.blue;
                //Gizmos.DrawLine(u.transform.position, 
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
