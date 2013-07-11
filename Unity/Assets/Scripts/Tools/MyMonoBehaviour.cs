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

    public static T GetNearest<T>(T target, ICollection<T> list) where T:Component {

        if (target == null)
        {
            throw new System.ArgumentNullException("Target is null");
        }

        if (list == null)
        {
            throw new System.ArgumentNullException("List is null");
        }
                
        var e = list.GetEnumerator();
        if(!e.MoveNext()) {            
            return null;
        }

        T nearest = e.Current;
        float dMin = Vector3.Distance(target.transform.position, nearest.transform.position);
        
        while (e.MoveNext())
        {
            float d = Vector3.Distance(target.transform.position, e.Current.transform.position);
            if (d < dMin)
            {
                dMin = d;
                nearest = e.Current;
            }
        }

        return nearest;
    }
}
