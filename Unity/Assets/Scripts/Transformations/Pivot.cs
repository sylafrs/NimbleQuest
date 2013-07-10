using UnityEngine;
using System.Collections.Generic;

/**
  * @class Pivot
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Pivot : MonoBehaviour {
    public float range;
    public float distanceRatio;

    public void Update()
    {
        Vector3 scale = this.transform.localScale;
        scale.z = range;
        this.transform.localScale = scale;

        Vector3 pos = this.transform.localPosition;
        pos.z = distanceRatio * range;
        this.transform.localPosition = pos;
    }
}
