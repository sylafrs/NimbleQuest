using UnityEngine;
using System.Collections.Generic;

/**
  * @class InputManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class InputManager : MonoBehaviour {

    void Update()
    {
        const float epsilon = 0.05f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h <= -epsilon)
        {
            Game.OnDirectionKey(Orientation.WEST);
        }
        else if (h >= epsilon)
        {
            Game.OnDirectionKey(Orientation.EAST);
        }
        else if (v <= -epsilon)
        {
            Game.OnDirectionKey(Orientation.SOUTH);
        }
        else if (v >= epsilon)
        {
            Game.OnDirectionKey(Orientation.NORTH);
        }
        else
        {
            //Debug.Log("NO MOVE");
        }
    }

}
