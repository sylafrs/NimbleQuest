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
}
