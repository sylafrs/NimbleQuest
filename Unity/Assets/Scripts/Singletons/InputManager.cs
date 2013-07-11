using UnityEngine;
using System.Collections.Generic;

/**
  * @class InputManager
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class InputManager : MonoBehaviour {

    const float epsilon = 0.05f;
    public bool pauseState = false;

    void Update()
    {
        this.UpdatePosition();

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

    void UpdatePosition()
    {
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

        UpdateMouse();
    }

    Vector2? onPress;

    void UpdateMouse()
    {
        if (onPress == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                onPress = Input.mousePosition;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 delta = (Vector2)Input.mousePosition - (Vector2)onPress;                
                Orientation? o = fromDeltaScreen(delta);
                if(o != null)
                    Game.OnDirectionKey((Orientation)o);

                onPress = null;
            }
        }
    }

    public static Orientation? fromDeltaScreen(Vector2 delta)
    {
        if (delta == Vector2.zero)
            return null;

        float X = Mathf.Abs(delta.x);
        float Y = Mathf.Abs(delta.y);

        if (X > Y)
        {
            if (delta.x > 0)
            {
                return Orientation.EAST;
            }
            else
            {
                return Orientation.WEST;
            }
        }
        else
        {
            if (delta.y > 0)
            {
                return Orientation.NORTH;
            }
            else
            {
                return Orientation.SOUTH;
            }
        }  
    }
}
