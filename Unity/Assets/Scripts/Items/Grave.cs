using UnityEngine;
using System.Collections.Generic;

/**
  * @class Grave
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Grave : Item
{
    protected override void DoFunction()
    {
        Game.heroicGroup.KillLast();
    }   
}
