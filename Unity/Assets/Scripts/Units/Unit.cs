using UnityEngine;
using System.Collections.Generic;

using Orientation = OrientationAbsolute;

/**
  * @class Unit
  * @brief A simple and stupid Unit.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Unit : MonoBehaviour {

    private int health;
    public int Health
    {
        get
        {
            return this.health;
        }
    }

    private float remainingCooldown;

    public Orientation orientation;
    public int maxHealth;
    public int force;
    public Range range;
    public int cooldown;
    public Group group;
    
    public virtual bool Attack(Unit target)
    {
        if (this.remainingCooldown > 0)
        {
            return false;
        }

        target.health -= this.force;
        this.remainingCooldown = this.cooldown;

        return true;       
    }

    protected virtual void Update()
    {
        if (this.remainingCooldown > 0)
        {
            this.remainingCooldown -= Time.deltaTime;
        }

        UpdateRotation();
    }

    protected virtual void OnDying()
    {
        group.OnFellowDeath(this);
    }

    private void UpdateRotation()
    {
        this.transform.forward = Vector3.Slerp(this.transform.forward, OrientationUtility.ToVector3(this.orientation), Time.deltaTime);
    }

    public void OnDrawGizmos()
    {
        const int up = 5;
        const int len = 10;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            this.transform.position + (Vector3.up * up), 
            this.transform.position + (this.transform.forward * len) + (Vector3.up * up)
        );
    }
}
