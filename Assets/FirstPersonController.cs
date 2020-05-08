using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;

    /// <summary>
    /// The GameObject to rotate up and down. The root object will only rotate around the vertical axis
    /// </summary>
    public GameObject verticalRotationReciever;


    private Rigidbody body;
    void Start()
    {
        body = this.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseAiming();
        KeyboardMovement();
        //Press the space bar to apply no locking to the Cursor
        if (Input.GetKey(KeyCode.Space))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButton(0))
            Cursor.lockState = CursorLockMode.Locked;
    }

    void MouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;


        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);

        this.verticalRotationReciever.transform.localEulerAngles = new Vector3(-rotX, 0, 0);
    }

    void KeyboardMovement()
    {
        Vector3 dir = new Vector3(0, 0, 0);

        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        var rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        var rotated = rotation * dir;


        body.velocity = rotated * moveSpeed;

        //transform.Translate(dir * moveSpeed * Time.deltaTime);
    }
}
