using UnityEngine;
using System.Collections.Generic;

public enum AttackType
{
    KILL,
    DAMAGES,
    HEAL,
    SLOW
}

/**
  * @class Unit
  * @brief A simple and stupid Unit.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public abstract class Unit : MonoBehaviour {

    public AttackType type;

    public int force;
    public Range range;
    public float attackSpeed;
    public float cooldown;
    public int maxHealth;

    protected float speed;
    private float remainingCooldown;

    private int health;
    public int Health
    {
        get
        {
            return this.health;
        }
    }
    public int HealthPercent
    {
        get
        {
            if (this.maxHealth <= 0) return 0;
            return this.health / this.maxHealth;
        }
    }

    [HideInInspector]
    public Orientation orientation;    
    public Group group;
    
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

    protected virtual void Start()
    {
        this.health = this.maxHealth;
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

    protected float GetSpeed()
    {
        return Game.settings.speed * speed * Time.deltaTime;
    }

    private void MoveForward()
    {
        if (Game.settings.dontMoveHero && this is Hero) return;

        this.BeforeMoveForward();

        Vector3 forward = OrientationUtility.ToVector3(this.orientation);
        this.transform.position += forward * GetSpeed();
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

    protected virtual void BeforeMoveForward()
    {

    }

    //public void OnGUI()
    //{
    //    if (IsLeader)
    //    {
    //        this.group.OnGUI();
    //    }
    //}
}
