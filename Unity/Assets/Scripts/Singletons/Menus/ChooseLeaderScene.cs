using UnityEngine;
using System.Collections.Generic;

/**
  * @class ChooseLeaderScene
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class ChooseLeaderScene : MonoBehaviour {

    private Hero selected;

    private void Awake()
    {
        selected = null;
    }

    private void OnGUI()
    {
        if (Game.started)
        {
            Color old = GUI.color;        

            GUILayout.Label("Choose hero : ");
            GUILayout.BeginHorizontal();
            foreach(var u in Game.settings.heroesPrefabs) {
                if (u == selected) GUI.color = Color.red;
                if (GUILayout.Button(u.name))
                {
                    selected = u;
                }
                GUI.color = old;
            }
            GUILayout.EndHorizontal();
        }

        if (selected != null)
        {
            if (GUILayout.Button("Validate"))
            {
                Game.LaunchGame(selected);
            }
        }
    }


}
