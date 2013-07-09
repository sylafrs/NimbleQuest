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

    private FollowAtOrtho lifebar;
    private Transform innerLifebar;
    private float timeRemainingShowingLife;
    
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
        this.AttachLifeBar();        
    }

    private void AttachLifeBar()
    {
        GameObject parent = GameObject.Find("GUI/Lifebars");
        Camera guiCam = GameObject.FindGameObjectWithTag("GUICamera").camera;
        
        GameObject lifeBarGO = GameObject.Instantiate(Game.settings.lifeBarPrefab) as GameObject;
        if (lifeBarGO == null)
        {
            throw new UnityException("Can't find the life bar prefab");
        }

        lifebar = lifeBarGO.GetComponent<FollowAtOrtho>();
        if (lifebar == null)
        {
            throw new UnityException("Can't find FollowAtOrtho component");
        }

        lifebar.transform.parent = parent.transform;
        lifebar.target = this.transform;
        lifebar.cam = guiCam;

        innerLifebar = lifeBarGO.transform.FindChild("Inner");

        // this.SetLifebar(); // No need for this
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
        if (!Game.started)
            return;

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

        this.UpdateLifebar();
    }

    public void SetLifebar()
    {
        float p = this.HealthPercent;

        Vector3 life = innerLifebar.transform.localScale;
        life.x = p;
        innerLifebar.transform.localScale = life;

        Renderer lb = innerLifebar.transform.FindChild("Model").renderer;
        lb.material.color = Color.Lerp(Color.red, Color.green, p);

        timeRemainingShowingLife = Game.settings.timeShowingLife;
    }

    public void UpdateLifebar()
    {
        const float FADE_FINISHED = -10;

        if (timeRemainingShowingLife > 0)
        {
            timeRemainingShowingLife -= Time.deltaTime;
        }
        else if (timeRemainingShowingLife != FADE_FINISHED)
        {
            Renderer lb;
            Color c;
            float alpha;
            
            // Fade the innerbar
            lb = innerLifebar.transform.FindChild("Model").renderer;
            c = lb.material.color;
            alpha = c.a;

                // Reduce the alpha channel
                alpha = Mathf.Lerp(alpha, 0, Time.deltaTime);
                if (alpha < 0.1f)
                {
                    alpha = 0;
                    timeRemainingShowingLife = FADE_FINISHED;
                }

            c.a = alpha;
            lb.material.color = c;

            // Apply the same alpha value to the background
            lb = lifebar.transform.FindChild("Model").renderer;
            c = lb.material.color;
            c.a = alpha; // use the same computed value
            lb.material.color = c;
        }
    }

    protected virtual void OnDying()
    {
        group.OnFellowDeath(this);
    }

    private void UpdateOrientation()
    {
        if (GetBaseSpeed() == 0)
        {
            this.orientation = this.group.leader.orientation;
            return;
        }

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

    public float GetBaseSpeed()
    {
        return speed;
    }

    protected float GetSpeed()
    {
        if(IsInGroup)
            return Game.settings.speed * this.group.GetSpeed() * Time.deltaTime;

        return Game.settings.speed * this.speed * Time.deltaTime;  
    }

    private void MoveForward()
    {
        if (Game.settings.dontMoveHero && this is Hero) return;

        this.BeforeMoveForward();

        Vector3 forward = OrientationUtility.ToVector3(this.orientation);
        this.transform.position += forward * GetSpeed();
    }

    public void UpdatePosition()
    {
        if (this.speed > 0)
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
    }

    public virtual void OnDrawGizmos()
    {
        if (Game.started && Game.settings.GizmosType == GIZMOSTYPE.RANGE)
        {
            Range.OnDrawGizmos(this);
        }
    }

    protected virtual void OnTriggerExit(Collider trigger) {
        if(trigger.name.Equals("Walls")) {
            Game.OnUnitHitsWall(this);
        }
    }

    protected virtual void BeforeMoveForward() { /* Nothing by default */ }
    

    //public void OnGUI()
    //{
    //    if (IsLeader)
    //    {
    //        this.group.OnGUI();
    //    }
    //}
}
