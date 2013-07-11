using UnityEngine;
using System.Collections.Generic;

/**
  * @class Freeze
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Freeze : Item
{
    public int power = 5;

    protected override void DoFunction()
    {
        foreach (MonsterGroup mg in Game.monsterGroups)
        {
            mg.leader.ReceiveAttack(AttackType.SLOW, power);
            foreach (Monster m in mg.fellows)
            {
                m.ReceiveAttack(AttackType.SLOW, power);
            }
        }
    }   
}
