using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 lastPos, worldVelocity;

    [SerializeField] private float speed, rotationSpeed, maxSpeed, maxRotationSpeed;
    private float fuel = 1;
    public float fuelWasteSpeed;

    private bool canRotate, isLanded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        DetectIfLanded();

        if (fuel != 0)
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position - transform.up * 0.85f, -transform.up, 0.1f);

        Transform hitTransform = hit.transform;

        if (hitTransform == null)
        {
            isLanded = false;
            return;
        }

        float heading = Mathf.Abs(((transform.position - hitTransform.position).normalized - transform.up).magnitude);

        if (hitTransform.gameObject.layer == 3 && rb.velocity.magnitude < 1.5f && rb.angularVelocity < 0.8f && heading < 1f)
        {
            isLanded = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Rigidbody2D otherRB = other.transform.GetComponent<Rigidbody2D>();

        float speedDifference;

        if (otherRB != null) speedDifference = Mathf.Abs((otherRB.velocity - worldVelocity).magnitude);
        else speedDifference = Mathf.Abs((other.transform.parent.GetComponent<OrbitingObjController>().velocity - worldVelocity).magnitude);

        if (speedDifference > 16) HudController.Die();
        if (speedDifference > 6 && !isLanded)
        {
            Vector2 outVector = (transform.position - other.transform.position).normalized;

            rb.AddForce(outVector * 100);
        }
    }
}
