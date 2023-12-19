using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))] 
public class PaperGenerationComponent : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3[] vertices;
    int[] tris;
    Mesh mesh;
    const int verticesCount = 8; //Paper
    bool generated = false;
    private void Awake()
    {
        mesh = new Mesh();
        vertices = GenerateVertices();
        mesh.vertices = vertices;
        mesh.triangles = GenerateTris();
        mesh.uv = GenerateUVs();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        generated = true;
    }
    private Vector3[] GenerateVertices()
    {
        Vector3[] vertices = new Vector3[verticesCount]
        {
            //Recto
            new Vector3(0,0,0),//0
            new Vector3(2,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(2,1,0),//3

            //Verso
            new Vector3(0,0,0),//4
            new Vector3(2,0,0),//5
            new Vector3(0,1,0),//6
            new Vector3(2,1,0),//7
        };
        return vertices;
    }
    private int[] GenerateTris()
    {
        int[] tris = new int[12]
        {
            //Recto
            0, 2, 1, 
            1, 2, 3,

            //Verso
            5, 6, 4,
            7, 6, 5
        };
        return tris;
    }

    private Vector2[] GenerateUVs()
    {
        Vector2[] UVs = new Vector2[]
        {
            //Recto
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0),
            


            //Verso
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)

        };

        return UVs;
    } 

    // Update is called once per frame
    void Update()
    {
        if (generated)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                float waveOffset = Mathf.Sin(Time.time * 2f + i) * 0.1f;
                vertices[i] = new Vector3(vertices[i].x, vertices[i].y, waveOffset);
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }
}