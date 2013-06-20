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
        this.leader = leader;
        this.fellows = new List<Unit>();
    }

    public void AddFellow(Unit fellow)
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

        this.fellows.Add(fellow);
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
