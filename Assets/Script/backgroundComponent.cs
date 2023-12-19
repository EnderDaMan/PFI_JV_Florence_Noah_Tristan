using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundComponent : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] public List<GameObject> Obstacles;

    private Transform groundTransform;
    private List<GameObject> spawnedObstacles = new List<GameObject>();

    private float timePassed;
    private float timeSinceSpeedInc = 10;

    private void Start()
    {
        if (transform.Find("Grid"))
            groundTransform = transform.Find("Grid").Find("Ground");
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        timeSinceSpeedInc -= Time.deltaTime;

        if (timeSinceSpeedInc <= 0 && speed < 8)
        {
            speed += .1f;
            if (timePassed >= 120)
                speed += 1f;

            timeSinceSpeedInc = 10;
        }

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.name == "Graveyard" && transform.position.x <= -17.95f)
            TeleportAndSpawn("Graveyard", 17.95f);

        if (transform.name == "Mountains" && transform.position.x <= -13.43f)
            TeleportAndSpawn("Mountains", 13.43f);

        if (transform.name == "Ground" && transform.position.x <= -17.50f)
            TeleportAndSpawn("Ground", 19.90f);
    }

    float minDistanceBetweenObstacles = 3.5f;

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
            int maxObstacles = Mathf.Min(Mathf.FloorToInt(timePassed / 30f) + 1, 4); // Max obstacles per chunk, gradually increasing
            int obstacleCount = Random.Range(1, maxObstacles + 1); // Random variation

            for (int i = 0; i < obstacleCount; i++)
            {
                int randomIndex = Random.Range(0, Obstacles.Count);
                GameObject randomObstacle = Instantiate(Obstacles[randomIndex]);

                randomObstacle.transform.parent = groundTransform;

                // Randomize spawn position with minimum distance constraint
                Vector3 spawnPosition = GetRandomSpawnPosition();
                while (IsTooCloseToOtherObstacles(spawnPosition))
                {
                    spawnPosition = GetRandomSpawnPosition();
                }

                randomObstacle.transform.position = spawnPosition;
                randomObstacle.transform.rotation = Quaternion.identity;

                Transform skelly = randomObstacle.transform.Find("Skeleton");
                if (skelly)
                {
                    skelly.position = new Vector3(skelly.position.x,skelly.position.y,GameObject.FindGameObjectWithTag("Player").transform.position.z);
                }

                // Add the spawned obstacle to the list
                spawnedObstacles.Add(randomObstacle);
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float spawnX = Random.Range(transform.position.x - 2f, transform.position.x + 7f);
        float spawnY = -2;
        float spawnZ = 0;

        return new Vector3(spawnX, spawnY, spawnZ);
    }

    bool IsTooCloseToOtherObstacles(Vector3 position)
    {
        foreach (var obstacle in spawnedObstacles)
        {
            if (Vector3.Distance(position, obstacle.transform.position) < minDistanceBetweenObstacles)
            {
                return true;
            }
        }
        return false;
    }
}
