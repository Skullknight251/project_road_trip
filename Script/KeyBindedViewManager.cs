using UnityEngine;
using UnityEngine.UI;

public class KeyBindedViewManager : MonoBehaviour
{
    public static KeyBindedViewManager Instance { get; private set; }
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Button quitButton;
    public bool isKeyBindedViewToggle;
    void Start()
    {
        Hide();
        isKeyBindedViewToggle = false;
        quitButton.onClick.AddListener(() =>
        {
            Hide();
            MainMenuManager.Instance.Show();
        });

        
    }
    public void Show()
    {
        scrollbar.Select();
        gameObject.SetActive(true);
    }

    void Awake()
    {
        Instance = this;
    }
    public void Toggle()
    {
        isKeyBindedViewToggle = !isKeyBindedViewToggle;

        if (isKeyBindedViewToggle)
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
