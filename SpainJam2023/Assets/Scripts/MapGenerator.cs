using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject sunPrefab_, asteroidPrefab_;

    private Rigidbody2D playerRB;

    private Transform playerTransform, sunsTransform, asteroidsTransform;

    private float playerAcomulatedMovement, randomAsteroidGenerationMoment = 20;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        sunsTransform = GameObject.Find("Suns").transform;
        asteroidsTransform = GameObject.Find("Asteroids").transform;

        playerRB = playerTransform.GetComponent<Rigidbody2D>();

        // Make suns generate at a minimum distance of 900 u and maximum of 1300 from each other and inside sunsTransform.
    }

    // Update is called once per frame
    void Update()
    {
        AsteroidGenerationProcess();
    }

    void AsteroidGenerationProcess()
    {
        if (HudController.dead) return;

        playerAcomulatedMovement += playerRB.velocity.magnitude * Time.deltaTime;

        if (playerAcomulatedMovement >= randomAsteroidGenerationMoment)
        {
            playerAcomulatedMovement = 0;

            randomAsteroidGenerationMoment = Random.Range(10, 30);

            GenerateAsteroid();
        }
    }

    void GenerateAsteroid()
    {
        float randomAngleGeneration = Random.Range(0, 360);
        Vector2 asteroidPos = new Vector2(playerTransform.position.x + Mathf.Cos(randomAngleGeneration) * 340, playerTransform.position.y + Mathf.Sin(randomAngleGeneration) * 340);

        Instantiate(asteroidPrefab_, asteroidPos, Quaternion.identity, asteroidsTransform);
    }
}
