using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThrowState
{
    WindingUp,
    Idle,
    Throwing
}

[RequireComponent(typeof(Rigidbody))]
public class ThrowingController : MonoBehaviour
{

    public GameObject throwingArm;
    public GameObject[] ballPrefabs;
    public float windupSpeed = 1;
    /// <summary>
    /// Speed with which the arm returns back to its stable state after throwing
    /// </summary>
    public float throwSpeed = 1;
    public float windupAngle = 30;

    /// <summary>
    /// The maximum speed the ball will launch at when the arm is fully retracted
    /// </summary>
    public float maxLaunchSpeed = 5;
    public float launchAngleFromHorizontal = 30;

    public float maxTiltAngle = 10f;

    private ThrowState state = ThrowState.Idle;

    /// <summary>
    /// Percentage of the way through the throw cycle the arm is currently at [0, 1]
    /// </summary>
    private float throwingArmProgressionFactor;
    /// <summary>
    /// Percentage of the way through the throw cycle the arm is currently at [0, 1]
    /// </summary>
    private float throwingArmTiltProgressionFactor;

    /// <summary>
    /// the rotation of the throwing arm when the component is instantiated. used to reset the position back when throwing is complete
    /// </summary>
    private float baseRotation;

    /// <summary>
    /// factor of how far the arm wound up in the windup phase. [0, 1]
    /// </summary>
    private float woundupEnergy;

    /// <summary>
    /// The tilt of the arm on this particular windup-release cycle
    /// </summary>
    private float currentTilt;

    private GameObject spawnedBall;
    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        baseRotation = throwingArm.transform.localEulerAngles.x;
        rigidBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ThrowState.Idle:
                if(Input.GetAxisRaw("Fire1") != 0)
                {
                    spawnBall();
                    throwingArmTiltProgressionFactor = 0;
                    currentTilt = Random.Range(-maxTiltAngle, maxTiltAngle);
                    state = ThrowState.WindingUp;
                }
                break;
            case ThrowState.WindingUp:
                if (Input.GetAxisRaw("Fire1") == 0)
                {
                    woundupEnergy = throwingArmProgressionFactor;
                    state = ThrowState.Throwing;
                    break;
                }
                throwingArmTiltProgressionFactor = throwingArmProgressionFactor = Mathf.Clamp(throwingArmProgressionFactor + (windupSpeed / 100) * Time.deltaTime, 0, 1);
                break;
            case ThrowState.Throwing:
                throwingArmProgressionFactor = Mathf.Clamp(throwingArmProgressionFactor - (throwSpeed * woundupEnergy / 100) * Time.deltaTime, 0, 1);
                if(throwingArmProgressionFactor == 0)
                {
                    releaseBall();
                    woundupEnergy = 0;
                    state = ThrowState.Idle;
                }
                break;
        }
        setThrowingArmTransform();
    }

    private void setThrowingArmTransform()
    {
        throwingArm.transform.localEulerAngles = new Vector3(
            baseRotation + Mathf.LerpAngle(0, windupAngle, throwingArmProgressionFactor),
            Mathf.LerpAngle(0, currentTilt, throwingArmTiltProgressionFactor),
            throwingArm.transform.localEulerAngles.z);
    }

    private void spawnBall()
    {
        var prefab = this.ballPrefabs[Random.Range(0, this.ballPrefabs.Length)];
        spawnedBall = Instantiate(prefab, throwingArm.transform);
        spawnedBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void releaseBall()
    {

        var speed = maxLaunchSpeed * woundupEnergy * woundupEnergy;
        var angleAboveHorizontal = launchAngleFromHorizontal * (Mathf.PI / 180);

        var direction = new Vector3(0, Mathf.Sin(angleAboveHorizontal), Mathf.Cos(angleAboveHorizontal));

        
        var localScale = spawnedBall.transform.localScale;
        spawnedBall.transform.parent = transform.parent;

        spawnedBall.transform.localScale = localScale;

        var rigidBody = spawnedBall.GetComponent<Rigidbody>();
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.velocity = throwingArm.transform.TransformDirection(direction) * speed
            + this.rigidBody.velocity;
    }
}
