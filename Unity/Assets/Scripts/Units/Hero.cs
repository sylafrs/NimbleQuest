using UnityEngine;
using System.Collections.Generic;

/**
  * @class Hero
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Hero : Unit {
    public void Start()
    {
        this.speed = Game.settings.heroSpeed;
    }
}
