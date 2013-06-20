using UnityEngine;
using System.Collections.Generic;

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

    public int maxHealth;
    public int force;
    public Range range;
    public int cooldown;

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
    }
}
