using UnityEngine;
using System.Collections.Generic;

/**
  * @class Group
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Group {

    public List<Unit> fellows;
    public Unit leader;

    public Group(Unit leader)
    {
        if (leader == null)
        {
            throw new System.ArgumentNullException("Leader is null");
        }

        this.leader = leader;
        this.leader.group = this;
        this.fellows = new List<Unit>();
    }

    public Group AddFellow(Unit fellow)
    {
        if (fellow == null)
        {
            throw new System.ArgumentNullException("Fellow is null");
        }
        if(this.fellows.Contains(fellow)) 
        {
            throw new System.ArgumentException("Fellow already within the group");
        }
        if (this.leader == fellow)
        {
            throw new System.ArgumentException("Fellow is the leader !");
        }

        fellow.group = this;
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

    public void OnFellowDeath(Unit fellow)
    {
        if (fellow == null)
        {
            throw new System.ArgumentNullException("Fellow is null");
        }

        if (fellow == leader)
        {
            // GameOver
            Game.OnLeaderDeath();
        }
        
        if(this.fellows.Contains(fellow))
        {
            // Remove it from group
        }

        throw new System.ArgumentException("Fellow in not in the group !");
    }
}
