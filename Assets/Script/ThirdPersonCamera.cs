using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    //Script pris de l'examen final pratique.

    [SerializeField] Transform player;
    float rotationCamera = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = Mouse.current.delta.ReadValue() * Time.deltaTime * 75;

        rotationCamera -= delta.y;
        rotationCamera = Mathf.Clamp(rotationCamera, -90, 90);
        transform.localRotation = Quaternion.Euler(rotationCamera, 0, 0);

        player.Rotate(Vector3.up * delta.x);
    }
}
