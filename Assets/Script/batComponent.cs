using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class batComponent : MonoBehaviour
{
    [SerializeField] int imagesNB = 2;

    Mesh mesh;

    float frame = 0;
    float elapsedTime = 0;
    float cooldown = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        mesh = new();
        mesh.vertices = GenerateVertices();
        mesh.triangles = GenerateTriangles();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
    private Vector3[] GenerateVertices()
    {
        return new Vector3[]
        {
            new Vector3(0, 0),
            new Vector3(0, 1),
            new Vector3(1, 0),
            new Vector3(1, 1)
        };
    }

    private int[] GenerateTriangles()
    {
        return new int[]
        {
            0, 1, 2,
            1, 3, 2
        };
    }

    private Vector2[] GenerateUVS()
    {
        float delta = 1f / imagesNB;

        return new Vector2[]
        {
            new Vector2(frame * delta, 0),
            new Vector2(frame * delta , 1),
            new Vector2((frame + 1) * delta, 0),
            new Vector2((frame + 1) * delta, 1)
        };
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= cooldown)
        {
            mesh.vertices = GenerateVertices();
            mesh.uv = GenerateUVS();
            frame = (frame + 1) % imagesNB;
            elapsedTime -= cooldown;
            if (transform.position.x >= 12)
                transform.position = new Vector3(- 10, transform.position.y, transform.position.z);
            transform.Translate(Vector3.right * 0.5f);
        }
    }
}
