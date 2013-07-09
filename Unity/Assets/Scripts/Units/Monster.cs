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
    private float TimeRemainingPriority;

    public bool Racist = false;
    public bool BossOnly = false;
    public bool Weak = false;
    public bool Unique = false;
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

        this.TimeRemaining = 0;
        this.TimeRemainingPriority = 0;
    }
    
    protected override void BeforeMoveForward()
    {
        if (IsLeader)
        {
            bool CanForward = true;

            List<Orientation> orientations = this.possibleOrientations();
            int nOrientations = orientations.Count;

            if (nOrientations != 0)
            {
                CanForward = orientations.Contains(this.orientation);
            }
            
            TimeRemaining -= Time.deltaTime;
            TimeRemainingPriority -= Time.deltaTime;
            if (TimeRemainingPriority < 0)
            {
                if (FixedOrientationChangement != -1 && TimeRemaining < 0)
                {
                    bool right = (Random.Range(0, 2) == 0);

                    if(right)
                        this.group.NewOrientation(OrientationUtility.TurnRight(this.orientation));
                    else
                        this.group.NewOrientation(OrientationUtility.TurnLeft(this.orientation));

                    TimeRemaining = FixedOrientationChangement;
                    TimeRemainingPriority = Game.settings.minMonsterRotationTime;
                }
                else if ((!CanForward || TimeRemaining < 0) && nOrientations != 0)
                {
                    Orientation o = orientations[
                        Random.Range(0, nOrientations)
                    ];

                    this.group.NewOrientation(o);

                    TimeRemaining = Random.Range(3, 8);             
                    TimeRemainingPriority = Game.settings.minMonsterRotationTime;
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
        
    public Vector3 GetDangerEscapeDirection()
    {
        List<Unit> inDanger = this.InDangerFieldUnits(false, false);
        Vector3 average = Vector3.zero;
        int nUnits = inDanger.Count;

        foreach (var u in inDanger)
        {
            average += (this.transform.position - u.transform.position);
        }

        if (nUnits != 0)
        {
            average /= nUnits;
        }

        return average; // or Vector3.zero if no units 
    }

    public List<Orientation> possibleOrientations()
    {       
        List<Orientation> result = new List<Orientation>();
        Vector3 ave = this.GetDangerEscapeDirection();

        // No danger.
        if (ave == Vector3.zero)
        {
            Orientation f, l, r;
            f = this.orientation;
            l = OrientationUtility.TurnLeft(f);
            r = OrientationUtility.TurnRight(f);

            if (!this.CheckWall(f))
                result.Add(f);

            if (!this.CheckWall(r))
                result.Add(r);

            if (!this.CheckWall(l))
                result.Add(l);
        }

        // Danger
        else
        {
            float X = Mathf.Abs(ave.x);
            float Z = Mathf.Abs(ave.z);

            // Left or right if we are not going at left or at rigth
            if (X > Z && (this.orientation != Orientation.EAST && this.orientation != Orientation.WEST))
            {                
                if (ave.x > 0)
                {
                    if (!this.CheckWall(Orientation.EAST))
                        result.Add(Orientation.EAST);
                }
                else if (ave.x < 0)
                {
                    if (!this.CheckWall(Orientation.WEST))
                        result.Add(Orientation.WEST);
                }

            }

            // Top or bottom if we are not going at top or at bottom
            else if (this.orientation != Orientation.NORTH && this.orientation != Orientation.SOUTH)
            {
                if (ave.z > 0)
                {
                    if (!this.CheckWall(Orientation.NORTH))
                        result.Add(Orientation.NORTH);
                }
                else if (ave.z < 0)
                {
                    if (!this.CheckWall(Orientation.SOUTH))
                        result.Add(Orientation.SOUTH);
                }
            }

            // There is a little problem : we must take risks
            //
            // Forward if not wall or left/right (taking danger average into account)
            else
            {
                // Right or left
                if (X > Z)
                {
                    if (ave.x > 0)
                    {
                        if (!this.CheckWall(Orientation.EAST) && this.orientation == Orientation.EAST)
                        {
                            result.Add(Orientation.EAST);
                        }
                        else
                        {
                            if (!this.CheckWall(Orientation.NORTH))
                                result.Add(Orientation.NORTH);

                            if (!this.CheckWall(Orientation.SOUTH))
                                result.Add(Orientation.SOUTH);
                        }
                    }
                    else if (ave.x < 0)
                    {
                        if (!this.CheckWall(Orientation.WEST) && this.orientation == Orientation.WEST)
                        {
                            result.Add(Orientation.WEST);
                        }
                        else
                        {
                            if (!this.CheckWall(Orientation.NORTH))
                                result.Add(Orientation.NORTH);

                            if (!this.CheckWall(Orientation.SOUTH))
                                result.Add(Orientation.SOUTH);
                        }
                    }
                }
                else
                {
                    if (ave.z > 0)
                    {
                        if (!this.CheckWall(Orientation.NORTH) && this.orientation == Orientation.NORTH)
                        {
                            result.Add(Orientation.NORTH);
                        }
                        else
                        {
                            if (!this.CheckWall(Orientation.WEST))
                                result.Add(Orientation.WEST);

                            if (!this.CheckWall(Orientation.EAST))
                                result.Add(Orientation.EAST);
                        }
                    }
                    else if (ave.z < 0)
                    {
                        if (!this.CheckWall(Orientation.SOUTH) && this.orientation == Orientation.SOUTH)
                        {
                            result.Add(Orientation.SOUTH);
                        }
                        else
                        {
                            if (!this.CheckWall(Orientation.WEST))
                                result.Add(Orientation.WEST);

                            if (!this.CheckWall(Orientation.EAST))
                                result.Add(Orientation.EAST);
                        }
                    }
                }
            }
        }

        if (result.Count == 0)
        {
            Orientation f, l, r;
            f = this.orientation;
            l = OrientationUtility.TurnLeft(f);
            r = OrientationUtility.TurnRight(f);

            if (!this.CheckWall(f))
                result.Add(f);

            if (!this.CheckWall(r))
                result.Add(r);

            if (!this.CheckWall(l))
                result.Add(l);

            // Ne jamais aller tout droit dans ce cas...
            if (result.Count == 0)
            {
                result.Add(r);
            }
        }

        return result;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (Game.settings != null)
        {
            if (Game.settings.GizmosType == GIZMOSTYPE.MOVE)
            {
                if (this.IsLeader && this.speed != 0)
                {
                    Gizmos.color = Color.green;

                    var orientations = this.possibleOrientations();
                    if (orientations.Count == 0)
                    {
                        orientations.Add(this.orientation);
                    }
                    foreach (var o in orientations)
                    {
                        Vector3 dir = OrientationUtility.ToVector3(o) * 50;
                        Gizmos.DrawLine(this.transform.position, this.transform.position + dir);
                    }
                }

                //List<Unit> danger = this.InDangerFieldUnits();
                Vector3 ave = this.GetDangerEscapeDirection();

                if (ave == Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.red;
                    if (this.IsLeader && this.speed != 0)
                    {
                        Gizmos.DrawLine(this.transform.position, this.transform.position + (ave * 4));
                    }
                }

                Gizmos.DrawWireSphere(this.transform.position, Game.settings.securityDistance);
            }
        }
    }
    
    public static bool InDangerField(Unit a, Unit b)
    {
        float d = Vector3.Distance(a.transform.position, b.transform.position);
        return d < Game.settings.securityDistance;
    }

    public List<Unit> InDangerFieldUnits(bool includeHeros = false, bool includeMyFellows = false)
    {
        List<Unit> list = new List<Unit>();
        
        foreach (var mg in Game.monsterGroups)
        {
            if (includeMyFellows || mg != this.group)
            {
                if (InDangerField(this, mg.leader))
                {
                    list.Add(mg.leader);
                }

                foreach (var f in mg.fellows)
                {
                    if (InDangerField(this, f))
                    {
                        list.Add(f);
                    }
                }
            }
        }

        if (includeHeros)
        {
            if (InDangerField(this, Game.hg.leader))
            {
                list.Add(Game.hg.leader);
            }

            foreach (var f in Game.hg.fellows)
            {
                if (InDangerField(this, f))
                {
                    list.Add(f);
                }
            }
        }

        return list;
    }

    public override List<Group> GetEnemies()
    {
        List<Group> g = new List<Group>();
        g.Add(Game.hg);
        return g;
    }
}
