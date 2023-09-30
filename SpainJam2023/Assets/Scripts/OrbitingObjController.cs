using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingObjController : MonoBehaviour
{
    [SerializeField] private List<Sprite> planetSprites;
    [SerializeField] private List<Color> planetColors;

    public float orbitRadius;
    private float objSize, orbitingSpeed, orbitWidthMultiplyer, orbitHeightMultiplayer, anglePos;

    private bool imMoon;

    public Vector2 velocity;

    private Transform playerTransform;
    private Rigidbody2D playerRB;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        playerRB = playerTransform.GetComponent<Rigidbody2D>();

        // Randomize in function of object's conditions the Speed, the Size, the rotation, the orbit variations, the Starting angle, the sprite and the color.
        CreateStats();

        if(!imMoon) MapGenerator.planetsToGenerate -= 1;
    }

    void FixedUpdate()
    {
        Vector2 lastPos = transform.position;

        ChangePos();

        // Updating the Velocity variable.

        Vector2 v2pos = transform.position;
        velocity = (v2pos - lastPos) / Time.fixedDeltaTime;
    }

    void ChangePos()
    {
        anglePos += orbitingSpeed * Time.deltaTime;

        Vector2 actualPos = new Vector2(CalculateX(), CalculateY()) * orbitRadius;

        transform.localPosition = actualPos;
    }

    float CalculateX()
    {
        float x = Mathf.Cos(anglePos) * orbitWidthMultiplyer;

        return x;
    }

    float CalculateY()
    {
        float y = Mathf.Sin(anglePos) * orbitHeightMultiplayer;

        return y;
    }

    public void makeTargetPlanet()
    {
        transform.localScale /= objSize;
        objSize = 70;
        transform.localScale *= objSize;

        transform.GetComponent<GravityObject>().G *= 2;

        MapGenerator.planetsToGenerate -= 1;

        Debug.Log("MadeTargetPlanet");
    }

    void CreateStats()
    {
        orbitingSpeed = Random.Range(7.5f, 20f);
        if (Random.value > 0.5f) orbitingSpeed *= -1;

        objSize = Random.Range(26f, 40f);

        if (transform.parent.GetComponent<OrbitingObjController>()) imMoon = true;

        if (imMoon)
        {
            objSize /= 5;

            transform.GetComponent<Collider2D>().enabled = false;
            orbitRadius = 37;
        }
        else transform.GetComponent<CircleCollider2D>().radius *= objSize;

        orbitingSpeed /= orbitRadius;

        transform.GetComponent<SpriteRenderer>().size *= objSize;
        transform.GetChild(0).GetComponent<CircleCollider2D>().radius *= objSize;

        orbitWidthMultiplyer = Random.Range(0.9f, 1.2f);
        orbitHeightMultiplayer = Random.Range(0.8f, 1.1f);

        anglePos = Random.Range(0, 360);

        SpriteRenderer mySpriteRenderer = transform.GetComponent<SpriteRenderer>();

        int sprite = Random.Range(0, planetSprites.Count);
        mySpriteRenderer.sprite = planetSprites[sprite];

        int color = Random.Range(0, planetColors.Count);
        mySpriteRenderer.color = planetColors[color];

        transform.rotation = new Quaternion(0, 0, Random.Range(0, 360), 0);
    }

    // To avoid strange collision and unrealistic physics when player lands.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == playerTransform && playerTransform.parent == null)
        {
            playerTransform.parent = transform;
            playerRB.velocity -= velocity;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == playerTransform && playerTransform.parent == transform)
        {
            playerTransform.parent = null;
            playerRB.velocity += velocity;
        }
    }
}
