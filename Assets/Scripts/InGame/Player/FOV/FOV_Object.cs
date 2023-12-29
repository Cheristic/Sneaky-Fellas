using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FOV_Object : NetworkBehaviour
{

    [SerializeField] private float fov = 90f;
    [SerializeField] private int rayCount = 2;
    [SerializeField] float viewDistance = 50f;
    [SerializeField] private LayerMask layerMask;

    private Vector3 origin;
    private Mesh mesh;
    private float startingAngle;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            float angleRad = angle * Mathf.Deg2Rad;
            Vector3 angleVec = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            RaycastHit2D rayHit = Physics2D.Raycast(origin, angleVec, viewDistance, layerMask);
            if (rayHit.collider == null)
            {
                //No hit
                vertex = origin + angleVec * viewDistance;
            } else {
                //Hit
                vertex = rayHit.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }
    public void SetAimDirection(Vector3 aimDir)
    {
        float n = Mathf.Atan2(aimDir.normalized.y, aimDir.normalized.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        startingAngle = n+fov/2f;
    }
}
