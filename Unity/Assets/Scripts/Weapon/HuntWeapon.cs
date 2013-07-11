using UnityEngine;
using System.Collections.Generic;

/**
  * @class HuntWeapon
  * @brief Always touch the target.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class HuntWeapon : Weapon
{
    protected float ellapsedTime;
    private Vector3 initPosition;

    public override void Fire(Transform target)
    {
        GameObject bullet = GameObject.Instantiate(this.gameObject) as GameObject;
        HuntWeapon rw = bullet.GetComponent<HuntWeapon>();

        bullet.transform.parent = GameObject.Find("Units/Bullets").transform;

        rw.speed                = this.speed;
        rw.distance             = this.distance;
        rw.onHit                = this.onHit;
        rw.onTargetReached      = this.onTargetReached;
        rw.target               = target;
        rw.ellapsedTime         = 0;
        rw.initPosition         = this.transform.position;

        bullet.SetActive(true);
    }
        
    void Update()
    {
        if (Game.state != Game.State.PLAYING)
            return;

        if (target == null)
        {
            Hit();
            return;
        }

        ellapsedTime += Time.deltaTime;
        this.transform.position = Vector3.Lerp(initPosition, target.transform.position, ellapsedTime / speed);

        if (Vector3.Distance(this.transform.position, target.position) <= distance)
        {
            TargetReached();
        }
    }

    protected override void TargetReached()
    {
        base.TargetReached();
    }

    protected override void Hit()
    {
        GameObject.Destroy(this.gameObject);
        base.Hit();
    }
}