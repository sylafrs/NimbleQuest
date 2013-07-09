using UnityEngine;
using System.Collections.Generic;

/**
  * @class Hero
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Hero : Unit {
    protected override void Start()
    {
        if (Game.started)
        {
            this.speed = Game.settings.heroSpeed;
            base.Start();
        }
    }

    public override List<Group> GetEnemies()
    {
        List<Group> monsters = new List<Group>();
        foreach (var mg in Game.monsterGroups)
            monsters.Add(mg);

        return monsters;
    }
}
