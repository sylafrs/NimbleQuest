using UnityEngine;
using UnityEditor;

/**
  * @class CreatePlaneMesh
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class CreatePlaneMesh {
    [MenuItem("Tools/Create a plane")]
    public static void CreatePlane()
    {
        CreatePlane(PlayerSettings.defaultScreenWidth / 3.84f, PlayerSettings.defaultScreenHeight / 3.84f);
    }

    public static Mesh CreatePlanedMesh(float w, float h)
    {
        float W = w / 2.0f;
        float H = h / 2.0f;

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { 
                new Vector3(-W, -H, 0),
                new Vector3(W, -H, 0),
                new Vector3(W, H, 0),
                new Vector3(-W, H, 0)            
            };
        mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        mesh.uv = new Vector2[] {  
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
        };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    public static GameObject CreatePlane(float w, float h)
    {
        GameObject g = new GameObject("myPlane");
        MeshFilter m = g.AddComponent<MeshFilter>();
        m.mesh = CreatePlanedMesh(w, h);

        g.AddComponent<MeshRenderer>();
        g.AddComponent<BoxCollider>();

        g.transform.position = Vector3.zero;
        g.transform.rotation = Quaternion.identity;
        g.transform.localScale = Vector3.one;

        return g;
    }
}
