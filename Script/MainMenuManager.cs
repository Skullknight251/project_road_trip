using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }
    [SerializeField] private Button StartButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button KeyBindedButton;
    private CarController car;

    public void Awake()
    {
        Instance = this;
    }


    public void Start()
    {
        Show();
        StartButton.onClick.AddListener(() => {
            StartGame();
        });
        
        SettingButton.onClick.AddListener(() => {
            Hide();
            SettingManager.Instance.isSettingToggled = true;
            SettingManager.Instance.Show();
        });

        KeyBindedButton.onClick.AddListener(() => {
            Hide();
            KeyBindedViewManager.Instance.isKeyBindedViewToggle = true;
            KeyBindedViewManager.Instance.Show();
        });
        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        car = GameManager.Instance.GetCar();
    }

    public void StartGame()
    {
        MenuManager.Instance.Hide();
        MenuManager.Instance.GetCarController().SetCanControl(true);
    }

    public void Show()
    {
        StartButton.Select();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
