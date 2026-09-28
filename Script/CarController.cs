using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    [SerializeField] public Transform engineSpot;
    public event EventHandler OnStateChange;
    [SerializeField] public CarSO carSO;

    public Transform cameraSpot;
    [SerializeField] private float moveAcceleration = 30;
    [SerializeField] private float brakeAcceleration = 50;
    [SerializeField] private float turnSensitivity = 1f;
    [SerializeField] private float maxSteerAngle = 30;
    [SerializeField] private float idleThreshold = 10f;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float brakeMultiply;
    [SerializeField] private float accelMultiplier;
    private float velocity;
    private bool canControl;

    public enum Axel
    {
        Front,
        Rear
    }

    public enum State
    {
        Idle,
        ForwardLowSpeed,
        ForwardHighSpeed,
        ForwardMaxSpeed,
        Reverse
    }

    [Serializable]
    public struct Wheel
    {
        public Axel axel;
        public Transform mesh;
        public GameObject wheelEffect;
        public ParticleSystem smoke;
        public WheelCollider collider;
    }

    [SerializeField] private List<Wheel> wheels;
    private float throttle;
    private float brake;
    private State state ;
    private Vector3 massCenter;
    private Rigidbody rb;
    private CarLight carLight;
    private Vector2 inputVector;
    private float MoveInput => inputVector.y;
    private float SteeringInput => inputVector.x;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        carLight = GetComponent<CarLight>();
        rb.centerOfMass = massCenter;
        state = State.Idle;
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    public void Update()
    {
        GetInput();
        UpdateState();
        WheelEffect();
    }

    public void Awake()
    {
        
    }
    public void LateUpdate()
    {
        throttle = GameInput.Instance.GetAcceleration();
        brake = GameInput.Instance.GetBrake();
        Move();
        Brake();    
    }
    public void Move()
    {
        if (canControl)
        {
            foreach (Wheel wheel in wheels)
            {
                if (wheel.axel == Axel.Front)
                {
                    float steeringAngle = maxSteerAngle * turnSensitivity * SteeringInput;

                    wheel.collider.steerAngle = Mathf.Lerp(
                        wheel.collider.steerAngle,
                        steeringAngle,
                        0.6f);
                }

                wheel.collider.motorTorque = MoveInput * accelMultiplier * (moveAcceleration + moveAcceleration * throttle) * Time.deltaTime;
                //Debug.Log(throttle);

                UpdateRotation(wheel.collider, wheel.mesh);
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            foreach (Wheel wheel in wheels)
            {
                wheel.collider.motorTorque = 0;
                wheel.collider.brakeTorque = Mathf.Infinity;

                if (wheel.axel == Axel.Front)
                    wheel.collider.steerAngle = 0;

                UpdateRotation(wheel.collider, wheel.mesh);
            }
        }
    }
    public State GetState()
    {
        return state;
        
    }


    public void GetInput()
    {
        inputVector = GameInput.Instance.GetInputVectorNormalized();
    }


    public void UpdateRotation(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position,out rotation);

        transform.position = position;
        transform.rotation = rotation;  
    }

    public void UpdateState()
    {
        
        velocity = Vector3.Dot(rb.linearVelocity, transform.forward) * 3.6f;
        //Debug.Log(velocity.ToString());
        State newState;
        if (Mathf.Abs(velocity) < idleThreshold)
        {
            newState = State.Idle;
        }
        else if (velocity < 0 && MoveInput < 0)
        {
            newState = State.Reverse;
        }
        else if (velocity < minSpeed )
        {
            newState = State.ForwardLowSpeed;
        }
        else if (velocity < maxSpeed)
        {
            newState = State.ForwardHighSpeed;
        }
        else
        {
            newState = State.ForwardMaxSpeed;
        }

        if (newState != state)
        {
            state = newState;

            if(state == State.Reverse)
            {
                carLight.StartReverseBlink();
                Debug.Log("Blink");
            }
            else
            {
                carLight.StopReverseBlink();
            }
            OnStateChange?.Invoke(this, EventArgs.Empty);
            
        }
    }
    public float GetVelocity()
    {
        return velocity;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
    
    public void Brake()
    {
        if (state == State.Reverse)
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.collider.brakeTorque = (brake > 0) ? (brakeAcceleration + brakeAcceleration * brake) * brakeMultiply * Time.deltaTime : 0;
            }
            return;
        }
        if (brake > 0 || Mathf.Approximately(MoveInput, 0f))
        {
            foreach(Wheel wheel in wheels)
            {
                wheel.collider.brakeTorque = (brakeAcceleration + brakeAcceleration * brake) * brakeMultiply * Time.deltaTime;
                //Debug.Log(brake);
            }
            carLight.isBackLightOn = true;
            carLight.OperateBackLight();
        }
        else
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.collider.brakeTorque = 0;
            }
            carLight.isBackLightOn = false;
            carLight.OperateBackLight();
        }
        
            
    }
    public void SetCanControl(bool value)
    {
        canControl = value;
    }
    public void WheelEffect()
    {
        foreach (Wheel wheel in wheels)
        {
            if (brake > 0 && wheel.axel == Axel.Rear && wheel.collider.isGrounded == true && rb.linearVelocity.magnitude > 10f)
            {
                wheel.wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smoke.Emit(1);
            }
            else
            {
                wheel.wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
}