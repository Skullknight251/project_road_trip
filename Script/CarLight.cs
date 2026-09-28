using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CarLight : MonoBehaviour
{
    public enum HorizontalSide
    {
        Left, 
        Right 
    };

    public enum VerticalSide { 
        Front,
        Back
    }
    [Serializable]
    public struct Light
    {
        public GameObject lightObj;
        public Material material;
        public VerticalSide verticalSide;
        public HorizontalSide horizontalSide;
    }
    [SerializeField] private float blinkInterval = 0.4f;
    private bool isFrontLightOn;
    public bool isBackLightOn;
    public List<Light> lights;
    public Color FrontOnColor;
    public Color FrontOffColor;
    public Color BackOnColor;
    public Color BackOffColor;
    private Coroutine reverseBlinkCoroutine;
    private bool reverseLightState;
    public void Start()
    {
        isFrontLightOn = true;
        isBackLightOn = false;
        GameInput.Instance.OnLightTurnOnOrOff += GameInput_OnLightTurnOnOrOff;
    }

    public void OnDestroy()
    {
        GameInput.Instance.OnLightTurnOnOrOff -= GameInput_OnLightTurnOnOrOff;
    }
    private void GameInput_OnLightTurnOnOrOff(object sender, EventArgs e)
    {
        OperateFrontLight();
    }

    public void OperateFrontLight()
    {
        isFrontLightOn = !isFrontLightOn;

        if (isFrontLightOn) {
            foreach (Light light in lights) {
                if (light.verticalSide == VerticalSide.Front && light.lightObj.activeInHierarchy == false) {
                    light.lightObj.SetActive(true);
                    light.material.color = FrontOnColor; 
                }
            }
        }
        else
        {
            foreach (Light light in lights)
            {
                if (light.verticalSide == VerticalSide.Front && light.lightObj.activeInHierarchy == true)
                {
                    light.lightObj.SetActive(false);
                    light.material.color = FrontOffColor;
                }
            }
        }
    }

    public void OperateBackLight()
    {
       
        if (isBackLightOn)
        {
            foreach (Light light in lights)
            {
                if (light.verticalSide == VerticalSide.Back && light.lightObj.activeInHierarchy == false)
                {
                    light.lightObj.SetActive(true);
                    light.material.color= BackOnColor;
                }
            }
        }
        else
        {
            foreach (Light light in lights)
            {
                if (light.verticalSide == VerticalSide.Back && light.lightObj.activeInHierarchy == true)
                {
                    light.lightObj.SetActive(false);
                    light.material.color= BackOffColor;
                }
            }
        }
    }
    public void StartReverseBlink()
    {
        Debug.Log("Start Blink");
        if (reverseBlinkCoroutine != null)
            return;

        reverseBlinkCoroutine = StartCoroutine(ReverseBlink());
    }
    public void StopReverseBlink()
    {
        if (reverseBlinkCoroutine != null)
        {
            StopCoroutine(reverseBlinkCoroutine);
            reverseBlinkCoroutine = null;
        }

        reverseLightState = false;
        isBackLightOn = false;
        OperateBackLight();
    }
    private IEnumerator ReverseBlink()
    {
        while (true)
        {
            reverseLightState = !reverseLightState;

            isBackLightOn = reverseLightState;
            OperateBackLight();

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
