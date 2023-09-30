using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    static public int planetsToGenerate;

    [SerializeField] private GameObject sunPrefab_, asteroidPrefab_;

    private SunsBuilder sunsBuilder;

    private Rigidbody2D playerRB;

    private Transform playerTransform, sunsTransform, asteroidsTransform, targetPlanetTransform, arrowTransform;

    private float playerAcomulatedMovement, randomAsteroidGenerationMoment = 20;

    private bool generationProcessEnded;

    private GameObject sunFar;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        sunsTransform = GameObject.Find("Suns").transform;
        asteroidsTransform = GameObject.Find("Asteroids").transform;
        arrowTransform = GameObject.Find("Arrow").transform;

        playerRB = playerTransform.GetComponent<Rigidbody2D>();

        sunsBuilder = new SunsBuilder();
        sunsBuilder.ColliderArea = GetComponent<Collider2D>();
        sunsBuilder.PosFirst = sunsTransform.position;

        List<Vector3> listSuns = sunsBuilder.GenerateCircularSuns(500);
        Vector3 posFar = sunsBuilder.IdentifyRandomFarSun();

        planetsToGenerate += 1;

        GenerateSuns(listSuns, posFar);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGenerationEnded();

        if (HudController.dead || !generationProcessEnded) return;

        AsteroidGenerationProcess();

        UpdateArrow();
    }

    void GenerateSuns(List<Vector3> listSuns, Vector3 posFar)
    {
        for (int idx = 1; idx < listSuns.Count; idx++)
        {
            GameObject sunInstance = Instantiate(sunPrefab_, sunsTransform);
            sunInstance.transform.position = listSuns[idx];
            if (listSuns[idx] == posFar)
            {
                sunInstance.GetComponent<SpriteRenderer>().color = Color.red;
                sunFar = sunInstance;
            }
        }
    }

    void CheckIfGenerationEnded()
    {
        if (generationProcessEnded) return;
        if (planetsToGenerate == 1) targetPlanetTransform = sunFar.GetComponent<SunController>().GetLastPlanet();
        if (planetsToGenerate > 0) return;

        PlayerController.targetPlanetTransform = targetPlanetTransform;

        SunController.generationProcessEnded = true;

        generationProcessEnded = true;

        Debug.Log("GenerationEnded");
    }

    void UpdateArrow()
    {
        Vector2 dirToTarget = targetPlanetTransform.position - playerTransform.position;
        arrowTransform.up = dirToTarget;

        arrowTransform.position = playerTransform.position;
    }

    void AsteroidGenerationProcess()
    {
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == playerTransform)
        {
            HudController.Die();
        }
    }
}
