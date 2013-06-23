using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
  * @class UnitsEditor
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class UnitsWindow : EditorWindow
{
    [MenuItem("Tools/Create the units window")]
    public static UnitsWindow Create()
    {
        UnitsWindow w = EditorWindow.GetWindow(typeof(UnitsWindow)) as UnitsWindow;
        return w;
    }

    Dictionary<Unit, bool> toogleUnit;

    public void OnEnable()
    {
        if (toogleUnit == null)
            toogleUnit = new Dictionary<Unit, bool>();
    }

    Vector2 scroll = new Vector2();

    public void OnGUI()
    {
        if (!EditorApplication.isPlaying || !Application.loadedLevelName.Equals("scene"))
            return;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.LabelField("====== HEROIC GROUP ======");

        DebugUnit(Game.hg.leader);
        foreach (var u in Game.hg.fellows)
        {
            DebugUnit(u);
        }

        foreach (var mg in Game.monsterGroups)
        {
            EditorGUILayout.LabelField("====== MONSTER GROUP ======");

            DebugUnit(mg.leader);
            foreach (var u in mg.fellows)
            {
                DebugUnit(u);
            }
        }
               
        EditorGUILayout.EndScrollView();
    }

    void DebugUnit(Unit u)
    {
        Color c = GUI.color;

        if (!toogleUnit.ContainsKey(u))
            toogleUnit.Add(u, true);

        if (toogleUnit[u] = EditorGUILayout.InspectorTitlebar(toogleUnit[u], u))
        {
            if (u.IsLeader)
            {
                if (u is Monster)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Type", "Boss Monster");
                }
                else
                {
                    GUI.color = Color.green;
                    EditorGUILayout.LabelField("Type", "Hero Leader");
                }
            }
            else
            {
                GUI.color = Color.yellow;
                if (u is Monster)
                {
                    EditorGUILayout.LabelField("Type", "Monster fellow");
                }
                else
                {
                    EditorGUILayout.LabelField("Type", "Hero fellow");
                }
            }

            GUI.color = c;

            float percent = u.HealthPercent;

            if (percent < 0.3f) GUI.color = Color.red;
            else if (percent < 0.6f) GUI.color = Color.yellow;
            else GUI.color = Color.green;

            string str = "";
            for (int i = 0; i < 10; i++)
            {
                if (percent >= 0.1f * i)
                {
                    str += "|";
                }
                else
                {
                    str += ".";
                }
            }

            EditorGUILayout.LabelField("Health", str);

            GUI.color = c;
        }
    }
}
