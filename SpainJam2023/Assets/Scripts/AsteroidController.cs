using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private List<Sprite> asteroidSprites;
    [SerializeField] private List<Color> asteroidColors;

    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        CreateStats();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }

    void CheckDistance()
    {
        if (HudController.dead) return;

        float distToPlayer = Mathf.Abs((playerTransform.position - transform.position).magnitude);

        if (distToPlayer > 500) Destroy(gameObject);
    }

    void CreateStats()
    {
        float dir = Random.Range(0, 360);
        float size = Random.Range(0.5f, 2.0f);
        Vector2 velocity = new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));

        transform.rotation = new Quaternion(0, 0, dir, 0);
        transform.localScale *= size;
        transform.GetComponent<Rigidbody2D>().velocity = velocity;

        SpriteRenderer mySpriteRenderer = transform.GetComponent<SpriteRenderer>();

        int sprite = Random.Range(0, asteroidSprites.Count);
        mySpriteRenderer.sprite = asteroidSprites[sprite];

        int color = Random.Range(0, asteroidColors.Count);
        mySpriteRenderer.color = asteroidColors[color];
    }
}
