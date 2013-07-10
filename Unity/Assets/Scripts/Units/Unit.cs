using UnityEngine;
using System.Collections.Generic;

public enum AttackType
{
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
public abstract class Unit : MyMonoBehaviour {

    public AttackType type;

    public int force;
    public Range range;
    public float attackSpeed;
    public float cooldown;
    public int maxHealth;
    public Weapon weapon;

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
    public float HealthPercent
    {
        get
        {
            if (this.maxHealth <= 0) return 0;
            return Mathf.Clamp01((float)this.health / this.maxHealth);
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
        if(this.weapon)
            this.weapon.Init(this);        
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
        if (target.health == 0 && type == AttackType.DAMAGES)
            return false;

        if (target.health == target.maxHealth && type == AttackType.HEAL)
            return false;

        this.remainingCooldown = this.cooldown;
                
        if (weapon)
        {
            weapon.OnDistanceLessThan(0.1f, () =>
            {
                target.ReceiveAttack(this.type, this.force);
            });

            weapon.Fire(target.transform);
            return true;
        }        

        Debug.Log("I (" + this.name + ") need a weapon");
        target.ReceiveAttack(this.type, this.force);
        return true;
    }

    public void ReceiveAttack(AttackType type, int force)
    {
        if (type == AttackType.DAMAGES)
        {
            this.health = Mathf.Clamp(this.health - force, 0, this.maxHealth); 
            Debug.Log("[RECEIVES DAMAGES]\nUnit : " + name + "\nDamages : " + force);
        }

        if (type == AttackType.HEAL)
        {            
            this.health = Mathf.Clamp(this.health + force, 0, this.maxHealth);
            Debug.Log("[RECEIVES HEAL]\nUnit : " + name + "\nRecovery : " + force);      
        }

        this.SetLifebar();  
    }

    protected virtual void Update()
    {  
        if (Game.state != Game.State.PLAYING)
            return;

        if (this.health == 0)
        {
            // Little cheat : undead mode :)
            if ((!(this is Hero) || !IsLeader && !Game.settings.undeadMode))
            {
                this.OnDying();
                return;
            }
        }

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

        this.UpdateWeapon();
        this.UpdateLifebar();

        this.CheckWall();
    }

    private void UpdateWeapon()
    {
        remainingCooldown -= Time.deltaTime;
        if (remainingCooldown < 0)
        {
            if (this.range.field == Range.FIELD.GROUP)
            {
                List<Unit> units = this.GetTargetsInRange();
                foreach (var u in units)
                {
                    if(this.Attack(u))
                        break; // Otherwise the healer is really too cheated !
                }
            }
            else
            {
                Unit u = this.GetNearestTargetInRange();
                if (u)
                {
                    this.Attack(u);
                }
            }
        }
    }

    public Unit GetNearestTargetInRange()
    {
        List<Unit> targets = this.GetTargetsInRange();
        Unit nearest = null;

        var e = targets.GetEnumerator();
        if(e.MoveNext()) 
        {
            float minDistance = Vector3.Distance(this.transform.position, e.Current.transform.position);
            nearest = e.Current;            

            while (e.MoveNext())
            {
                float distance = Vector3.Distance(this.transform.position, e.Current.transform.position);
                if (distance < minDistance)
                {
                    nearest = e.Current;
                    minDistance = distance;
                }
            }
        }

        return nearest;
    }

    public abstract List<Group> GetEnemies();

    public List<Group> GetTargets()
    {
        List<Group> g = null;

        if (this.range.field == Range.FIELD.GROUP)
        {
            g = new List<Group>();
            g.Add(this.group);
        }
        else
        {
            g = this.GetEnemies();
        }

        return g;
    }

    public List<Unit> GetTargetsInRange()
    {
        List<Unit> inRangeUnits = new List<Unit>();
        List<Group> targets = GetTargets();
        foreach (var g in targets)
        {
            if (Range.IsInRange(this, g.leader))
            {
                inRangeUnits.Add(g.leader);
            }

            foreach (var u in g.fellows)
            {
                if (Range.IsInRange(this, u))
                    inRangeUnits.Add(u);
            }
        }               

        return inRangeUnits;
    }

    private void SetLifebar()
    {        
        if (lifebar)
        {
            float p = this.HealthPercent;

            Vector3 life = innerLifebar.transform.localScale;
            life.x = p;
            innerLifebar.transform.localScale = life;

            Renderer lb = innerLifebar.transform.FindChild("Model").renderer;
            lb.material.color = Color.Lerp(Color.red, Color.green, p);

            lb = lifebar.transform.FindChild("Model").renderer;
            Color c = lb.material.color;
            c.a = 1; // use the same computed value
            lb.material.color = c;

            timeRemainingShowingLife = Game.settings.timeShowingLife;
        }
    }

    private void UpdateLifebar()
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

    public void OnLeaderDeath()
    {
        this.OnDying();
    }

    protected virtual void OnDying()
    {
        Debug.Log("[UNIT DEATH : " + this.name + "]");
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

    public void CheckWall()
    {
        Vector3 min, max;
        Game.GetMinMaxLevel(out min, out max);
        Rect zone = RectUtility.MinMaxRect(min, max);               

        if (!RectUtility.ContainsXZ(zone, this.transform.position))
        {
            Game.OnUnitHitsWall(this);
        }
    }
   
    protected virtual void BeforeMoveForward() { /* Nothing by default */ }

    public override void DestroyObject()
    {
        if(this.lifebar)
            GameObject.Destroy(this.lifebar.gameObject);

        base.DestroyObject();
    }

    //public void OnGUI()
    //{
    //    if (IsLeader)
    //    {
    //        this.group.OnGUI();
    //    }
    //}
}
