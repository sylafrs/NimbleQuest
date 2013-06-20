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
        ANGLE,
        FORWARD,
        GROUP
    }

    public FIELD field;
    public int angle;

    public bool infinite;
    public int range;
}
