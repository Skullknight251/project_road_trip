using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }

    [SerializeField] private Button quitButton;
    [SerializeField] private Slider effectVolumeSlider;
    [SerializeField] private TextMeshProUGUI effectVolumeValue;
    public bool isSettingToggled;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
        isSettingToggled = false;
        quitButton.onClick.AddListener(() =>
        {
            Hide();
            MainMenuManager.Instance.Show();
        });

        effectVolumeSlider.onValueChanged.AddListener(OnEffectVolumeChanged);
    }

    private void OnEffectVolumeChanged(float value)
    {
        SoundManager.Instance.SetEffectVolume(value);

        effectVolumeValue.text = Mathf.RoundToInt(value * 10).ToString();
    }

    public void Show()
    {

        float volume = SoundManager.Instance.GetVolume();
        effectVolumeSlider.Select();
        effectVolumeSlider.SetValueWithoutNotify(volume);
        effectVolumeValue.text = Mathf.RoundToInt(volume * 10).ToString();

        gameObject.SetActive(true);
    }
    public void Toggle()
    {
        isSettingToggled = !isSettingToggled;

        if (isSettingToggled)
        {
            Show();
        }
        else
        {
            Hide();
            MainMenuManager.Instance.Show();
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool isActive()
    {
        return gameObject.activeInHierarchy;
    }
}