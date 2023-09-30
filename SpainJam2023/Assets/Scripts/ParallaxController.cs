using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private Renderer myRend;

    private Vector2 lastPos;

    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        myRend = transform.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 v2pos = transform.position;
        Vector2 posOffset = (v2pos - lastPos) * Time.deltaTime * speed;

        ChangeOffset(posOffset);

        lastPos = transform.position;
    }

    void ChangeOffset(Vector2 posOffset)
    {
        myRend.material.mainTextureOffset += posOffset;
    }
}
