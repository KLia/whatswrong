using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class DrawCollider2D : MonoBehaviour
{
    PolygonCollider2D col;

    void Awake()
    {
        col = GetComponent<PolygonCollider2D>();
    }

    void OnDrawGizmos()
    {
        if (!col) col = GetComponent<PolygonCollider2D>();

        Gizmos.color = Color.green;

        for (int p = 0; p < col.pathCount; p++)
        {
            Vector2[] points = col.GetPath(p);

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 a = transform.TransformPoint(points[i]);
                Vector3 b = transform.TransformPoint(points[(i + 1) % points.Length]);

                Gizmos.DrawLine(a, b);
            }
        }
    }
}