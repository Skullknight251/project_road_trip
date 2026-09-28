using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class GameInput : MonoBehaviour
{
    private MyInputAction myInputAction;

    public static GameInput Instance {get; private set;}
    public event EventHandler OnHornActive;
    public event EventHandler OnInteractActive;
    public event EventHandler OnLightTurnOnOrOff;
    
    public void Awake()
    {
        myInputAction = new MyInputAction();
        Instance = this;
        myInputAction.Car.Enable();
        myInputAction.Car.Horn.performed += Horn_performed;
        myInputAction.Car.Interact.performed += Interact_performed;
        myInputAction.Car.Light.performed += Light_performed;
        myInputAction.Car.Escape.performed += Escape_performed;
    }

 

    private void Escape_performed(InputAction.CallbackContext obj)
    {
        if (!SettingManager.Instance.isActive() && !KeyBindedViewManager.Instance.isActive())
        {
            MenuManager.Instance.Toggle();
        }
        else if(SettingManager.Instance.isActive())
        {
            SettingManager.Instance.Toggle();
        }
        else
        {
            KeyBindedViewManager.Instance.Toggle();
        }
    }

    public float GetAcceleration()
    {
        return myInputAction.Car.Accelerate.ReadValue<float>();
    }

    public float GetBrake()
    {
        return myInputAction.Car.Brake.ReadValue<float>();
    }
    private void Light_performed(InputAction.CallbackContext obj)
    {
        OnLightTurnOnOrOff?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVectorNormalized()
    {
        Vector2 vector = myInputAction.Car.Move.ReadValue<Vector2>();
        vector = vector.normalized;
        return vector;
    }
    private void Horn_performed(InputAction.CallbackContext obj)
    {
        OnHornActive?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractActive?.Invoke(this, EventArgs.Empty);
    }
    
}
