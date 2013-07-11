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
    protected System.Action onTargetReached;
    protected System.Action onHit;

    public virtual void Init(Unit u) {
        this.speed = u.attackSpeed;
        this.distance = 0;
        this.onTargetReached = null;
        this.onHit = null;
    }
        
    public void OnDistanceLessThan(float distance, System.Action callback)
    {
        this.distance = distance;
        this.onTargetReached = callback;
    }
    
    public virtual void Fire(Transform target) { /* Nothing by default */ }

    protected virtual void Hit() {
        if (onHit != null)        
            onHit();        
    }

    public void OnHit(System.Action callback)
    {
        onHit = callback;
    }
    
    protected virtual void TargetReached()
    {
        Hit();
        if (onTargetReached != null)
            onTargetReached();
    }    
}
