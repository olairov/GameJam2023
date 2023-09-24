using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject sunPrefab_, asteroidPrefab_;

    private Rigidbody2D playerRB;

    private Transform playerTransform;

    private float playerAcomulatedMovement, randomAsteroidGenerationMoment;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        playerRB = playerTransform.GetComponent<Rigidbody2D>();

        randomAsteroidGenerationMoment = 27;

        // Make suns generate at a minimum distance of 900 u and maximum of 1300 from each other.       340
    }

    // Update is called once per frame
    void Update()
    {
        AsteroidGenerationProcess();
    }

    void AsteroidGenerationProcess()
    {
        playerAcomulatedMovement += playerRB.velocity.magnitude;

        if (playerAcomulatedMovement >= randomAsteroidGenerationMoment)
        {
            playerAcomulatedMovement = 0;
        }
    }

    void GenerateAsteroid()
    {

    }
}
