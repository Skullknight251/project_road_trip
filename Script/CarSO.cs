using UnityEngine;

[CreateAssetMenu()]
public class CarSO : ScriptableObject
{
    [SerializeField] private GameObject car;
    [SerializeField] private string name;
    [SerializeField] public SoundEffect_CarSO soundEffectCarSO;

    public CarController GetCarController()
    {
        return car.GetComponent<CarController>();
    }

}
