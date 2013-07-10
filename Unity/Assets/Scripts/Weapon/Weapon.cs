using UnityEngine;
using System.Collections.Generic;

/**
  * @class Weapon
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public abstract class Weapon : MonoBehaviour {

    protected Transform target;
    protected float speed;
    protected float distance;
    protected System.Action callback;

    public virtual void Init(Unit u) {
        this.speed = u.attackSpeed;
    }
        
    public void OnDistanceLessThan(float distance, System.Action callback)
    {
        this.distance = distance;
        this.callback = callback;
    }
    
    public virtual void Fire(Transform target) { /* Nothing by default */ }
    protected virtual void Hit() { /* Nothing by default */ }
    
    protected virtual void TargetReached()
    {
        Hit();
        if (callback != null)
            callback();
    }    
}
