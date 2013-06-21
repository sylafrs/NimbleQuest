using UnityEngine;
using System.Collections.Generic;

/**
  * @class MainMenuScene
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class MainMenuScene : MonoBehaviour {
    public void OnGUI()
    {
        if (GUILayout.Button("PLAY"))
        {
            Application.LoadLevel(1);
        }
        if (GUILayout.Button("QUIT"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
#else
            Application.Quit();
#endif
        }
    }
}
