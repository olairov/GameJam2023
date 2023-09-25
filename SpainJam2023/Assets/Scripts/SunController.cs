using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField] private GameObject planetPrefab_;

    private Transform playerTransform, myChilds;

    private float lightRange;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        myChilds = transform.GetChild(0);

        lightRange = Random.Range(1200, 1600);
        myChilds.GetChild(0).GetComponent<HardLight2D>().Range = lightRange;
        myChilds.GetChild(1).GetComponent<SpriteRenderer>().size *= lightRange;

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
        float distToPlayer = playerDir.magnitude;

        // Comprobar si el jugador está cerca para ahorrar recursos.

        if (distToPlayer > 2000)
        {
            myChilds.gameObject.SetActive(false);
            return;
        }
        else myChilds.gameObject.SetActive(true);

        RaycastHit2D rayToPlayer = Physics2D.Raycast(transform.position, playerDir);

        if (rayToPlayer.transform == playerTransform)
        {
            if (distToPlayer >= lightRange) distToPlayer = lightRange;

            HudController.lightExposure = 1 - distToPlayer / lightRange;

            if (transform.name == "Sun") Debug.Log("BLYYYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADDDDDDDDDDDD");
        }
        else HudController.lightExposure = 0;
    }

    void GeneratePlanets()
    {
        //Generating random num of planets and moons (and avoiding bugs with the light system).

        GameObject light = myChilds.GetChild(0).gameObject;

        int planetNum = Random.Range(4, 7);

        for (int i = 0; i < planetNum; i++)
        {
            Transform newPlanetTransform = Instantiate(planetPrefab_, myChilds).transform;

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

        if (other.transform.GetComponent<Rigidbody2D>() && other.transform != playerTransform)
        {
            Destroy(other.gameObject);
        }
    }
}
