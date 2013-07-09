using UnityEngine;
using System.Collections.Generic;

/**
  * @class FollowOffset
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class FollowOffset : MonoBehaviour {
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed;
    public float rotSmoothSpeed;
    public bool lookAt;

    public void Update()
    {
        if (target != null)
        {  
            this.transform.position = Vector3.Lerp(
                this.transform.position,
                target.transform.position + offset,
                Time.deltaTime * smoothSpeed
            );

            Quaternion prev = this.transform.rotation;

            if (lookAt)
            {
                this.transform.LookAt(target);
            }
            else
            {
                this.transform.localEulerAngles = new Vector3(90, 0, 0);
            }

            this.transform.rotation = Quaternion.Lerp(prev, this.transform.rotation, Time.deltaTime * rotSmoothSpeed);
        }
    }
}
