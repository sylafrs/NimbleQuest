using UnityEngine;
using System.Collections.Generic;

/**
  * @class Group
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Group {
    
    // When should we change our orientation ?
    public struct FellowPath {   
     
        public struct CheckPoint {
            public Vector3 position;
            public Orientation orientation;

            public override string ToString()
            {
                return "Change at " + this.position.ToString() + " to " + this.orientation.ToString();
            }

            public void Copy(CheckPoint toCopy)
            {
                this.position = toCopy.position;
                this.orientation = toCopy.orientation;
            }

            public CheckPoint Clone()
            {
                CheckPoint cp = new CheckPoint();
                cp.Copy(this);
                return cp;
            }
        }

        public Queue<CheckPoint> checkpoints;
        
        // Important : to use BEFORE adding the unit to fellow !
        public FellowPath(Group g)
        {
            if (g.leader == null)
            {
                throw new System.MissingFieldException("I need a leader, otherwise, it's useless !");
            }
            else
            {
                Unit last = g.lastFellow;
                if (last != null)
                {
                    this = g.fellowPaths[last].Clone();
                }
                else
                {
                    this.checkpoints = new Queue<CheckPoint>();
                }
            }
        }

        public void Copy(FellowPath toCopy)
        {
            this.checkpoints = new Queue<CheckPoint>();
            foreach (CheckPoint cp in toCopy.checkpoints)
            {
                this.checkpoints.Enqueue(cp.Clone());
            }
        }

        public FellowPath Clone()
        {
            FellowPath clone = new FellowPath();
            clone.Copy(this);
            return clone;
        }
    }

    public List<Unit> fellows;
    public Dictionary<Unit, FellowPath> fellowPaths;
    
    public Unit leader;

    public Unit lastFellow
    {
        get
        {
            int nFellow = this.fellows.Count;
            if (nFellow != 0)
            {
                return this.fellows[nFellow - 1];
            }

            return null;
        }
    }

    public Unit lastUnit
    {
        get
        {
            Unit last = lastFellow;
            if (last != null)
            {
                return last;
            }

            return this.leader;
        }
    }

    public Group(Unit leader)
    {
        if (leader == null)
        {
            throw new System.ArgumentNullException("Leader is null");
        }

        this.leader = leader;
        this.leader.group = this;
        this.fellows = new List<Unit>();
        this.fellowPaths = new Dictionary<Unit, FellowPath>();
    }

    public bool Contains(Unit unit)
    {
        if (unit == null)
            throw new System.ArgumentNullException("I need a unit !");

        if (unit == this.leader)
            return true;

        if (this.fellows.Contains(unit))
            return true;

        return false;
    }

    public Group AddFellow(Unit fellow)
    {
        if (fellow == null)
        {
            throw new System.ArgumentNullException("Fellow is null");
        }
        if(this.Contains(fellow)) 
        {
            throw new System.ArgumentException("Fellow already within the group");
        }
        if (this.leader == fellow)
        {
            throw new System.ArgumentException("Fellow is the leader !");
        }

        fellow.group = this;
        this.fellowPaths.Add(fellow, new FellowPath(this));
        this.fellows.Add(fellow);
        
        return this;
    }

    public int GetUnitPosition(Unit fellow)
    {
        if (fellow == null)
        {
            throw new System.ArgumentNullException("Fellow is null");
        }

        if (fellow == leader)
        {
            return 0;
        }

        int position = this.fellows.IndexOf(fellow);
        if (position == -1)
        {
            throw new System.ArgumentException("Fellow is not within the group");
        }

        return position+1;
    }

    public Unit GetUnitAtPosition(int index)
    {
        if (index < 0)
        {
            return null;
        }

        if (index == 0)
        {
            return this.leader;
        }

        index--;

        if (index >= fellows.Count)
        {
            return null;
        }

        return fellows[index];
    }

    public void OnFellowDeath(Unit fellow)
    {
        if (fellow == null)
        {
            throw new System.ArgumentNullException("Fellow is null");
        }

        if (fellow == this.leader)
        {
            this.OnLeaderDeath();
            return;
        }
        
        if(this.fellows.Contains(fellow))
        {
            // Remove it from group
        }

        throw new System.ArgumentException("Fellow in not in the group !");
    }

    public void OnLeaderDeath()
    {

    }

    public void OnLeaderOrientationChangement()
    {
        FellowPath.CheckPoint checkpoint = new FellowPath.CheckPoint();
        checkpoint.position = this.leader.transform.position;
        checkpoint.orientation = this.leader.orientation;

        foreach (Unit fellow in fellows)
        {
            FellowPath fellowPath = fellowPaths[fellow];
            fellowPath.checkpoints.Enqueue(checkpoint);
        }
    }

    public void NewOrientation(Orientation o)
    {
        if (this.leader.orientation != o && this.leader.orientation != OrientationUtility.Inverse(o))
        {
            this.leader.orientation = o;
            this.OnLeaderOrientationChangement();
        }
    }

    public void OnGUI()
    {
        foreach (Unit u in this.fellows)
        {
            FellowPath p = this.fellowPaths[u];
            GUILayout.BeginHorizontal();
    
            GUILayout.Label(u.name + " (" + p.checkpoints.Count + ")");

            if (p.checkpoints.Count > 0)
            {
                FellowPath.CheckPoint first = p.checkpoints.Peek();
                Debug.DrawLine(
                    first.position - OrientationUtility.ToVector3(first.orientation) * 100, 
                    first.position + OrientationUtility.ToVector3(first.orientation) * 100,
                    Color.red
                );

                foreach (FellowPath.CheckPoint c in p.checkpoints)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(c.orientation.ToString());
                    GUILayout.Label(c.position.ToString());
                    GUILayout.EndVertical();
                }
            }
    
            GUILayout.EndHorizontal();
        }
    }    
}
