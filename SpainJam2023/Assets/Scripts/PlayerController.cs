using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public Transform targetPlanetTransform;

    private Rigidbody2D rb;

    private ParticleSystem.EmissionModule emissionModule;

    private Vector2 lastPos, worldVelocity;

    [SerializeField] private float speed, rotationSpeed, maxSpeed, maxRotationSpeed;
    private float fuel = 1;
    public float fuelWasteSpeed;

    private bool canRotate, isLanded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        emissionModule = transform.GetChild(0).GetComponent<ParticleSystem>().emission;
    }

    private void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.5f && fuel > 0) emissionModule.enabled = true;
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.5f || fuel == 0) emissionModule.enabled = false;
    }

    void FixedUpdate()
    {
        DetectIfLanded();

        if (fuel != 0 || HudController.dead)
        {
            Move();

            Rotate();

            if (fuel > 0) fuel -= (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal")) / 4) * fuelWasteSpeed;
            else fuel = 0;
        }

        if (isLanded && fuel < 1) fuel += fuelWasteSpeed * 10;
        else if (isLanded && fuel > 1) fuel = 1;

        HudController.fuel = fuel;

        Vector2 v2pos = transform.position;
        worldVelocity = (v2pos - lastPos) / Time.fixedDeltaTime;
        lastPos = transform.position;
    }

    void Move()
    {
        float yMovement = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(0, yMovement * speed * Time.fixedDeltaTime);

        rb.AddRelativeForce(movement);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    void Rotate()
    {
        float rotation = Input.GetAxis("Horizontal") * -rotationSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(rb.angularVelocity) > maxRotationSpeed) canRotate = false;
        else canRotate = true;

        if (canRotate) rb.AddTorque(rotation);
    }

    void DetectIfLanded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - transform.up * 1.2f, -transform.up, 0.1f);

        Transform hitTransform = hit.transform;

        if (hitTransform == null)
        {
            isLanded = false;
            return;
        }

        float heading = Mathf.Abs(((transform.position - hitTransform.position).normalized - transform.up).magnitude);

        if (hitTransform.gameObject.layer == 3 && rb.velocity.magnitude < 1.5f && rb.angularVelocity < 1f && heading < 1f)
        {
            isLanded = true;

            if (hitTransform == targetPlanetTransform)
            {
                HudController.Win();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D otherRB = other.transform.GetComponent<Rigidbody2D>();

        float speedDifference;

        if (otherRB != null) speedDifference = Mathf.Abs((otherRB.velocity - worldVelocity).magnitude);
        else speedDifference = Mathf.Abs((other.transform.parent.GetComponent<OrbitingObjController>().velocity - worldVelocity).magnitude);

        if (speedDifference > 24) HudController.Die();
        if (speedDifference > 10 && !isLanded)
        {
            Vector2 outVector = (transform.position - other.transform.position).normalized;

            rb.AddForce(outVector * 100);
        }
    }
}
