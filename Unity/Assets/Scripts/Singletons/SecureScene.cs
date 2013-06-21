using UnityEngine;
using System.Collections.Generic;

/**
  * @class SecurityScene
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class SecureScene : MonoBehaviour {
    public void Awake()
    {
        if (!Game.started)
        {
            Application.LoadLevel(0);
        }
    }
}
