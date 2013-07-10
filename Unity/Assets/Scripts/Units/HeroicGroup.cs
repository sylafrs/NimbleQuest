using UnityEngine;
using System.Collections.Generic;

/**
  * @class HeroicGroup
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class HeroicGroup : Group {
    public HeroicGroup(Hero leader) : base(leader) { }

    public override void OnLeaderDeath()
    {
        base.OnLeaderDeath();
        Game.OnLeaderDeath();
    }
}
