using UnityEngine;
using System.Collections.Generic;

/**
  * @class MyMonoBehaviour
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class MyMonoBehaviour : MonoBehaviour {

    public virtual void DestroyObject()
    {
        GameObject.Destroy(this.gameObject);
    }
}
