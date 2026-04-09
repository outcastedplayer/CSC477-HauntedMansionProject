using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Screens")]
    public GameObject pauseMenu;
    public GameObject winScreen;
    public GameObject loseScreen;

    public UIState CurrentState { get; private set; } = UIState.HUD;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        HideAll();
        SetState(UIState.HUD);
    }

    public void SetState(UIState newState)
    {
        CurrentState = newState;
        if (pauseMenu) pauseMenu.SetActive(newState == UIState.PauseMenu);
        if (winScreen) winScreen.SetActive(newState == UIState.WinScreen);
        if (loseScreen) loseScreen.SetActive(newState == UIState.LoseScreen);
    }

    public void ShowPauseMenu() { SetState(UIState.PauseMenu); }
    public void HidePauseMenu() { SetState(UIState.HUD); }
    public void ShowWinScreen() { SetState(UIState.WinScreen); }
    public void ShowLoseScreen() { SetState(UIState.LoseScreen); }

    void HideAll()
    {
        if (pauseMenu) pauseMenu.SetActive(false);
        if (winScreen) winScreen.SetActive(false);
        if (loseScreen) loseScreen.SetActive(false);
    }
}
