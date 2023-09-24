using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;

    private Camera cameraComponent;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        cameraComponent = transform.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HudController.dead) return;

        // Update position

        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);

        // Zoom with space

        float cameraAugmentation = Mathf.Pow(Input.GetAxis("Jump") + 1, 2);
        cameraComponent.orthographicSize = 40 * cameraAugmentation;
    }
}
