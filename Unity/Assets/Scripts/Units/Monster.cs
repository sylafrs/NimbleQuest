using UnityEngine;
using System.Collections.Generic;

/**
  * @class Enemy
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Monster : Unit
{
    public float Speed;
    private float TimeRemaining;

    public bool Racist = false;
    public bool BossOnly = false;
    public bool Weak = false;
    public float FixedOrientationChangement = -1;

    protected override void Start()
    {
        //GameObject securityTrigger = new GameObject("SecurityTrigger");
        //SphereCollider sc = securityTrigger.AddComponent<SphereCollider>();
        //sc.radius = Game.settings.securityDistance;
        //sc.isTrigger = true;
        //sc.transform.parent = this.transform;
        //sc.transform.localPosition = Vector3.zero;

        this.speed = this.Speed;
        base.Start();
    }
    
    protected override void BeforeMoveForward()
    {
        if (IsLeader)
        {
            Orientation forward = this.orientation;
            Orientation left = OrientationUtility.TurnLeft(forward);
            Orientation right = OrientationUtility.TurnRight(forward);

            bool CanForward = !this.CheckWall(forward) && !this.CheckMonster(forward);
            bool CanLeft = !this.CheckWall(left) && !this.CheckMonster(left);
            bool CanRight = !this.CheckWall(right) && !this.CheckMonster(right);
            
            TimeRemaining -= Time.deltaTime;
            if (!CanForward || TimeRemaining < 0)
            {
                if (CanLeft && CanRight)
                {
                    int coin = Random.Range(0, 2);
                    if (coin == 0)
                        this.group.NewOrientation(left);
                    else
                        this.group.NewOrientation(right);
                }
                else if (CanLeft)
                {
                    this.group.NewOrientation(left);
                }
                else if (CanRight)
                {
                    this.group.NewOrientation(right);
                }

                if (FixedOrientationChangement != -1)
                {
                    TimeRemaining = FixedOrientationChangement;
                }
                else
                {
                    TimeRemaining = Random.Range(3, 8);
                }
            }
        }
    }

    private bool CheckWall(Orientation orientation)
    {
        // Get the level dimensions
        Vector3 min, max;
        Game.GetMinMaxLevel(out min, out max);
        Rect zone = RectUtility.MinMaxRect(min, max);

        // Add a little margin
        zone.x += Game.settings.securityMargin;
        zone.y += Game.settings.securityMargin;
        zone.width -= (Game.settings.securityMargin * 2);
        zone.height -= (Game.settings.securityMargin * 2);

        // Get the next direction
        Vector3 pos = this.transform.position;
        Vector3 forward = OrientationUtility.ToVector3(orientation);

        bool inZone = RectUtility.ContainsXZ(zone, pos + forward * this.GetSpeed());

        // Check if, after the movement, we'll hit the wall
        return (inZone == false);
    }

    private bool CheckMonster(Orientation orientation)
    {
        Vector3 forward = OrientationUtility.ToVector3(orientation);
        Vector3 position = this.transform.position + forward * this.GetSpeed();

        




        return false;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (Game.settings != null)
        {
            Gizmos.color = Color.red;        
            Gizmos.DrawWireSphere(this.transform.position, Game.settings.securityDistance);
        }
    }
}
