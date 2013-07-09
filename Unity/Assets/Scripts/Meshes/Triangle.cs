using UnityEngine;
using System.Collections.Generic;

/**
  * @class Triangle
  * @brief Description.
  * @author Sylvain Lafon
  * @see MonoBehaviour
  */
public class Triangle : MonoBehaviour {

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Create a triangle")]
    public static void CreateTriangle()
    {
        CreateTriangle(10);
    }
#endif

    public void Start()
    {
        this.GetComponent<MeshFilter>().mesh = CreateTriangleMesh(10);
    }

    public static GameObject CreateTriangle(float radius)
    {
        GameObject g = new GameObject("myTriangle");
        MeshFilter m = g.AddComponent<MeshFilter>();
        m.mesh = CreateTriangleMesh(radius);

        g.AddComponent<MeshRenderer>();
        g.AddComponent<MeshCollider>();
        g.AddComponent<Triangle>();

        g.transform.position = Vector3.zero;
        g.transform.rotation = Quaternion.identity;
        g.transform.localScale = Vector3.one;

        return g;
    }

    public static Mesh CreateTriangleMesh(float radius)
    {
        float side = (radius * 3) / Mathf.Sqrt(3);
        float altitude = (radius * 3) / 2;

        Vector3 center = Vector3.zero;
        Vector3 centerBase = center - Vector3.forward * (altitude - radius);

        Vector3 H = center + Vector3.forward * radius;
        Vector2 uvH = new Vector2(0.5f, 1);

        Vector3 A = centerBase - Vector3.right * (side / 2.0f);
        Vector2 uvA = new Vector2(0, 0);

        Vector3 B = centerBase + Vector3.right * (side / 2.0f);
        Vector2 uvB = new Vector2(1, 0);

        Vector3[] vertices = new Vector3[]{
            H, A, B
        };

        int[] triangles = new int[] {
            0, 2, 1
        };

        Vector2[] uvs = new Vector2[]{
            uvH, uvA, uvB
        };

        Mesh mesh = new Mesh();
        mesh.name = "Triangle";

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}
