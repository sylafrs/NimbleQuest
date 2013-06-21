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
    
    public void Update()
    {
        this.transform.position = Vector3.Lerp(
            this.transform.position, 
            target.transform.position + offset, 
            Time.deltaTime * smoothSpeed
        );
    }
}
