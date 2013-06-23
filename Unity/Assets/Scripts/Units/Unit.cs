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

    public Orientation orientation;
    public int maxHealth;
    public int force;
    public Range range;
    public int cooldown;
    public Group group;

    public float speed;

    public bool IsInGroup
    {
        get
        {
            return (group != null);
        }
    }

    public bool IsLeader
    {
        get
        {
            if (!IsInGroup)
            {
                return false;
            }

            return (this == group.leader);
        }
    }
    
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

        if (IsInGroup)
        {
            if (IsLeader)
            {
                MoveForward();
            }
            else
            {
                UpdatePosition();
                UpdateOrientation();                
            }
        }
    }

    protected virtual void OnDying()
    {
        group.OnFellowDeath(this);
    }

    private void UpdateOrientation()
    {
        Group.FellowPath path = this.group.fellowPaths[this];

        if (path.checkpoints.Count > 0)
        {
            Group.FellowPath.CheckPoint cp = path.checkpoints.Peek();

            bool change = false;
            switch (this.orientation)
            {
                case Orientation.EAST:
                    change = (this.transform.position.x >= cp.position.x);// || Mathf.Abs(this.transform.position.x - cp.position.x) <= epsilon);
                    break;                                               
                                                                         
                case Orientation.NORTH:                                  
                    change = (this.transform.position.z >= cp.position.z);// || Mathf.Abs(this.transform.position.z - cp.position.z) <= epsilon);
                    break;                                               
                                                                         
                case Orientation.SOUTH:                                  
                    change = (this.transform.position.z <= cp.position.z);// || Mathf.Abs(this.transform.position.z - cp.position.z) <= epsilon);
                    break;                                               
                                                                         
                case Orientation.WEST:                                   
                    change = (this.transform.position.x <= cp.position.x);// || Mathf.Abs(this.transform.position.x - cp.position.x) <= epsilon);
                    break;
            }

            if (change)
            {
                path.checkpoints.Dequeue();
                this.orientation = cp.orientation;
            }
        }
    }

    private void UpdateRotation()
    {
        this.transform.forward = Vector3.Slerp(this.transform.forward, OrientationUtility.ToVector3(this.orientation), Time.deltaTime * Game.settings.rotationSpeed);
    }

    private void MoveForward()
    {
        // if (this is Hero) return; // [DEBUG : Don't move our leader]

        Vector3 forward = OrientationUtility.ToVector3(this.orientation);
        this.transform.position += forward * Game.settings.speed * speed * Time.deltaTime;
    }

    private void UpdatePosition()
    {
        int position = this.group.GetUnitPosition(this);
        Unit previous = this.group.GetUnitAtPosition(position - 1);
        if (previous == null)
        {
            throw new System.InvalidOperationException("This guy is the leader");
        }

        // A remplacer par : if le checkpoint du précédent est le meme que le mien.
        if (previous.orientation == this.orientation)
        {
            Vector3 previousPosition = previous.transform.position;
            Vector3 space = OrientationUtility.ToVector3(this.orientation) * Game.settings.distanceUnits;
            Vector3 idealPosition = previousPosition - space;

            //this.transform.position = idealPosition;
            this.transform.position = Vector3.Lerp(this.transform.position, idealPosition, Time.deltaTime * Game.settings.fellowSmoothSpeed);
        }
        else
        {
            MoveForward();
        }
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

    protected virtual void OnTriggerExit(Collider trigger) {
        if(trigger.name.Equals("Walls")) {
            Game.OnUnitHitsWall(this);
        }
    }

    //public void OnGUI()
    //{
    //    if (IsLeader)
    //    {
    //        this.group.OnGUI();
    //    }
    //}
}
