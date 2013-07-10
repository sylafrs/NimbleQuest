using UnityEngine;
using System.Collections.Generic;

/**
  * @class MonsterGroup
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class MonsterGroup : Group {
    public MonsterGroup(Monster leader) : base(leader) { }

    public override void OnLeaderDeath()
    {
        base.OnLeaderDeath();
        Game.OnBossDeath(this);
    }
}
