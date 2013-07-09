using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
  * @class TriangleEditor
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
[CustomEditor(typeof(Triangle))]
public class TriangleEditor : Editor {

    Triangle t;

    public void OnEnable()
    {
        t = (Triangle)target;
    }

    public override void OnInspectorGUI()
    {
        if (t && GUILayout.Button("Create triangle"))
        {
            t.Start();
        }
    }


}
