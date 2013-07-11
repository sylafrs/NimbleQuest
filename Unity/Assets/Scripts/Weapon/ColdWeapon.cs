using UnityEngine;
using System.Collections.Generic;

/**
  * @class ColdWeapon
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class ColdWeapon : Weapon
{
    public Pivot pivot;
    float ellapsedTime;
    float realspeed;

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public override void Init(Unit u)
    {
        if (pivot)
        {
            pivot.range = u.range.distance;
            pivot.distanceRatio = Game.settings.distanceRatio / 2.0f;
        }

        base.Init(u);
    }    
    public override void Fire(Transform target)
    {
        this.realspeed = this.speed;
        if (this.speed == -1)
        {
            this.realspeed = 1;
            this.TargetReached();
        }

        this.transform.rotation = Quaternion.identity;
        this.ellapsedTime = 0;
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        this.ellapsedTime += Time.deltaTime;        
        if(this.speed != -1 && this.ellapsedTime >= this.speed) 
            TargetReached();
        else if(this.ellapsedTime >= this.realspeed)
            Hit();
        
        this.transform.eulerAngles = Vector3.Slerp(Vector3.zero, Vector3.up * 360, this.ellapsedTime / this.realspeed);
    }

    protected override void Hit()
    {
        if (this.ellapsedTime >= this.realspeed)
            this.gameObject.SetActive(false);

        base.Hit();
    }
}
