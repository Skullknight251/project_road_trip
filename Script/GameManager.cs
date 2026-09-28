using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static MenuManager;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<CarSO> carSOList;
    public static GameManager Instance { get; private set; }

    private string PLAYER_CAR = "player_car";
    private int carIndex = 0;
    private CarController currentCar;
    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        carIndex = PlayerPrefs.GetInt(PLAYER_CAR, 0);

        currentCar = Instantiate(
            carSOList[carIndex].GetCarController(),
            spawnPoint.position,
            spawnPoint.rotation);

        SoundManager.Instance.SetCar(currentCar);
        Camera.main.GetComponent<CameraMovement>().SetTarget(currentCar.transform);
        MenuManager.Instance.SetCar(currentCar);
        MenuManager.Instance.Show();
        MenuManager.Instance.CarChanged += MenuManager_CarChanged;
    }

    private void MenuManager_CarChanged(object sender, CarChangedEventArgs e)
    {
        if (currentCar != null)
        {
            spawnPoint.transform.position = currentCar.transform.position;
            Destroy(currentCar.gameObject);
            
            if (e.IsNextCar)
            {
                if (carIndex >= carSOList.Count-1 )
                {
                    carIndex = 0;
                }
                else
                {
                    carIndex++;
                }
            }
            else
            {
                if (carIndex == 0)
                {
                    carIndex = carSOList.Count - 1;
                }
                else
                {
                    carIndex--;
                }
            }
            currentCar = Instantiate(carSOList[carIndex].GetCarController(), spawnPoint.position, spawnPoint.rotation);

            SoundManager.Instance.SetCar(currentCar);
            MenuManager.Instance.SetCar(currentCar);
            PlayerPrefs.SetInt(PLAYER_CAR, carIndex);
            PlayerPrefs.Save();
        }
        
    }

    public CarController GetCar()
    {
        return currentCar;
    }

    void Update()
    {
        
    }

    public void StartGame()
    {

    }
}

