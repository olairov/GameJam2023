using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldController : MonoBehaviour
{
    private GravityObject gravityScript;

    // Start is called before the first frame update
    void Start()
    {
        gravityScript = transform.parent.GetComponent<GravityObject>();

        transform.GetComponent<CircleCollider2D>().radius = transform.parent.GetComponent<CircleCollider2D>().radius * 10;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Rigidbody2D>() && other.name != "Player")
        {
            gravityScript.objectsToFall.Add(other.transform);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() && other.name != "Player")
        {
            gravityScript.objectsToFall.Remove(other.transform);
        }
    }
}
