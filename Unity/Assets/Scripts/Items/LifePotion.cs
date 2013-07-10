using UnityEngine;
using System.Collections.Generic;

/**
  * @class LifePotion
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class LifePotion : Item
{
    protected override void DoFunction()
    {
        Game.heroicGroup.Heal();        
    }   
}
