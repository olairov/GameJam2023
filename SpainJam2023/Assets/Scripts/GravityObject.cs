using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField] private GameObject gravityFieldPrefab_;

    public List<Transform> objectsToFall;

    private Transform playerTransform;

    [SerializeField] private float G;

    void Start()
    {
        Instantiate(gravityFieldPrefab_, transform);

        playerTransform = GameObject.Find("Player").transform;

        objectsToFall = new List<Transform>();

        objectsToFall.Add(playerTransform);
    }

    void FixedUpdate()
    {
        //If player is dead it's erased from the list, and if that was the only component in the list it stops doing anything;

        if (HudController.dead && objectsToFall.Contains(playerTransform)) objectsToFall.Remove(objectsToFall[0]);
        if (objectsToFall.Count == 0) return;

        for (int i = 0; i < objectsToFall.Count; i++)
        {
            if (objectsToFall[i] != null) MakeThingsFall(objectsToFall[i]);
            else objectsToFall.Remove(objectsToFall[i]);
        }
    }

    void MakeThingsFall(Transform targetObjectTransform)
    {
        Rigidbody2D targetRB = targetObjectTransform.GetComponent<Rigidbody2D>();

        float distanceToTarget = Mathf.Abs((transform.position - targetObjectTransform.position).magnitude);

        // F = G * (m1*m2 / d^2)

        float attractionForce = G * (transform.GetComponent<SpriteRenderer>().size.x * targetObjectTransform.lossyScale.magnitude / Mathf.Pow(distanceToTarget, 2));

        Vector2 forceToTarget = (transform.position - targetObjectTransform.position).normalized * attractionForce * Time.fixedDeltaTime;

        targetRB.AddForce(forceToTarget);
    }
}
