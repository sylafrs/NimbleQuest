using UnityEngine;
using System.Collections.Generic;

/**
  * @class InputManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class InputManager : MonoBehaviour {

    public bool pauseState = false;

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
        //else
        //{
        //    //Debug.Log("NO MOVE");
        //}

        if (Input.GetAxis("Exit") > epsilon)
        {
            Game.OnExit();
        }

        if (Input.GetAxis("Pause") > epsilon)
        {
            if (pauseState == false)
            {
                pauseState = true;
                Game.OnPauseButton();
            }
        }
        else
        {
            pauseState = false;
        }

        // Debug mode
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
            {
                Game.settings.dontMoveHero = !Game.settings.dontMoveHero;
            }
        #endif
    }

}
