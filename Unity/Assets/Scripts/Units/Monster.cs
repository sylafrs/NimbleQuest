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

    protected override void Start()
    {
        this.speed = this.Speed;
        base.Start();
    }
    
    protected override void BeforeMoveForward()
    {
        while (CheckWall())
            ChangeOrientation();
    }

    private bool CheckWall()
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
        Vector3 forward = OrientationUtility.ToVector3(this.orientation);

        bool inZone = RectUtility.ContainsXZ(zone, pos + forward * this.GetSpeed());

        // Check if, after the movement, we'll hit the wall
        return (inZone == false);
    }

    private void ChangeOrientation()
    {
        this.orientation = OrientationUtility.TurnLeft(this.orientation);
    }
}
