using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] private Button NextCarButton;
    [SerializeField] private Button PreviousCarButton;
    private bool isMenuToggled;
    public event EventHandler<CarChangedEventArgs> CarChanged;
    public class CarChangedEventArgs : EventArgs
    {
        public bool IsNextCar;
    }
    private CarController carController;
    void Start()
    {
        isMenuToggled = true;
        NextCarButton.onClick.AddListener(() => {

            CarChanged?.Invoke(this, new CarChangedEventArgs
            {

                IsNextCar = true
            });
        });
        PreviousCarButton.onClick.AddListener(() => {

            CarChanged?.Invoke(this, new CarChangedEventArgs
            {
                IsNextCar = false
            });
        });

    }
    public void SetCar(CarController car)
    {
        carController = car;
    }
    public void Awake()
    {
        Instance = this;
    }
    public void Toggle()
    {
        isMenuToggled = !isMenuToggled;

        if (isMenuToggled) {
            Show();
        }
        else{
            Hide();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        carController.SetCanControl(false);
        Camera.main.GetComponent<CameraMovement>().GetMenuPosition();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Camera.main.GetComponent<CameraMovement>().SetTarget(GameManager.Instance.GetCar().transform);
    }

    public CarController GetCarController() { 
        return carController;   
    }
}
