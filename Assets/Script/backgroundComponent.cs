using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundComponent : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] public List<GameObject> Obstacles;

    private Transform groundTransform; // Reference to the Ground transform
    private List<GameObject> spawnedObstacles = new List<GameObject>(); // Keep track of spawned obstacles

    private void Start()
    {
        // Assuming the Ground is a child of the backgroundComponent
        if (transform.Find("Grid"))
            groundTransform = transform.Find("Grid").Find("Ground");
        
        if (groundTransform == null)
        {
            Debug.LogError("Ground not found as a child of backgroundComponent!");
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.name == "Graveyard" && transform.position.x <= -17.95f)
            TeleportAndSpawn("Graveyard", 17.95f);

        if (transform.name == "Mountains" && transform.position.x <= -13.43f)
            TeleportAndSpawn("Mountains", 13.43f);

        if (transform.name == "Ground" && transform.position.x <= -19.90f)
            TeleportAndSpawn("Ground", 19.90f);
    }

    void TeleportAndSpawn(string tilemapName, float tilemapWidth)
    {
        transform.position = new Vector3(transform.position.x + tilemapWidth * 2, transform.position.y, transform.position.z);

        // Remove old obstacles
        foreach (var obstacle in spawnedObstacles)
        {
            Destroy(obstacle);
        }
        spawnedObstacles.Clear();

        // Randomly spawn new obstacles
        if (tilemapName == "Ground" && Obstacles.Count > 0)
        {
            int randomIndex = Random.Range(0, Obstacles.Count);
            GameObject randomObstacle = Instantiate(Obstacles[randomIndex]);
            
            randomObstacle.transform.parent = groundTransform;

            randomObstacle.transform.position = GetRandomSpawnPosition();
            randomObstacle.transform.rotation = Quaternion.identity;

            // Add the spawned obstacle to the list
            spawnedObstacles.Add(randomObstacle);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Adjust the range of spawn positions based on your requirements
        float spawnX = Random.Range(transform.position.x, transform.position.x + 10f);
        float spawnY = -2;
        float spawnZ = 0;

        return new Vector3(spawnX, spawnY, spawnZ);
    }
}
