using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField] private GameObject planetPrefab_;

    private Transform playerTransform;

    private float lightRange;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        lightRange = Random.Range(1200, 1600);
        transform.GetChild(0).GetComponent<HardLight2D>().Range = lightRange;
        transform.GetChild(1).GetComponent<SpriteRenderer>().size *= lightRange;

        GeneratePlanets();
    }

    void Update()
    {
        CheckPlayerExposure();
    }

    void CheckPlayerExposure()
    {
        if (HudController.dead) return;

        // Throw raycast to player and if it touches it, depending on its distance, set higher or lower exposure.

        Vector2 playerDir = playerTransform.position - transform.position;

        RaycastHit2D rayToPlayer = Physics2D.Raycast(transform.position, playerDir);

        if (rayToPlayer.collider.transform == playerTransform)
        {
            float distToPlayer = playerDir.magnitude;
            if (distToPlayer >= lightRange) distToPlayer = lightRange;

            HudController.lightExposure = 1 - distToPlayer / lightRange;
        }
        else HudController.lightExposure = 0;
    }

    void GeneratePlanets()
    {
        //Generating random num of planets and moons (and avoiding bugs with the light system).

        GameObject light = transform.GetChild(0).gameObject;

        int planetNum = Random.Range(4, 7);

        for (int i = 0; i < planetNum; i++)
        {
            Transform newPlanetTransform = Instantiate(planetPrefab_, transform).transform;

            newPlanetTransform.GetComponent<OrbitingObjController>().orbitRadius = (i + 1) * 125f;
            if (Random.value > 0.65f) Instantiate(planetPrefab_, newPlanetTransform);
        }

        Instantiate(light, transform);
        Destroy(light);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == playerTransform)
        {
            HudController.Die();
        }
    }
}
