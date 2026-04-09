using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public FrontDoorController frontDoor;
    public CutsceneManager cutsceneManager;

    [Header("Settings")]
    public int keysRequired = 3;

    [Header("Events")]
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;
    public UnityEvent<GameState> OnStateChanged;

    public GameState CurrentState { get; private set; } = GameState.Exploring;
    public RoomType CurrentRoom { get; set; } = RoomType.Lobby;
    public int KeysCollected { get; private set; }
    public int PadlocksUnlocked { get; private set; }
    public bool IsPaused { get; private set; }

    InteractionSystem interactionSystem;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        interactionSystem = Camera.main?.GetComponent<InteractionSystem>();
        SetState(GameState.Exploring);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.Escaped || CurrentState == GameState.Trapped) return;
            TogglePause();
        }
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Exploring:
                SetCursorLocked(true);
                if (interactionSystem) interactionSystem.SetEnabled(true);
                break;

            case GameState.SolvingPuzzle:
                SetCursorLocked(false);
                if (interactionSystem) interactionSystem.SetEnabled(false);
                break;

            case GameState.AtFrontDoor:
                SetCursorLocked(true);
                if (interactionSystem) interactionSystem.SetEnabled(true);
                break;

            case GameState.Escaped:
                SetCursorLocked(false);
                if (interactionSystem) interactionSystem.SetEnabled(false);
                if (cutsceneManager) cutsceneManager.PlayWinCutscene();
                OnGameWon?.Invoke();
                break;

            case GameState.Trapped:
                SetCursorLocked(false);
                if (interactionSystem) interactionSystem.SetEnabled(false);
                if (cutsceneManager) cutsceneManager.PlayLoseCutscene();
                OnGameLost?.Invoke();
                break;
        }

        OnStateChanged?.Invoke(newState);
    }

    public void OnKeyCollected()
    {
        KeysCollected++;
        SoundManager.Instance?.PlaySFX("pickup");
    }

    public void OnPadlockUnlocked()
    {
        PadlocksUnlocked++;
        SoundManager.Instance?.PlaySFX("door_unlock");

        if (PadlocksUnlocked >= keysRequired)
            SetState(GameState.Escaped);
    }

    public void OnTimeExpired()
    {
        if (CurrentState == GameState.Escaped) return;
        SetState(GameState.Trapped);
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;

        if (IsPaused)
        {
            SetCursorLocked(false);
            UIManager.Instance?.ShowPauseMenu();
        }
        else
        {
            UIManager.Instance?.HidePauseMenu();
            if (CurrentState == GameState.Exploring)
                SetCursorLocked(true);
        }
    }

    void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    public void ReturnToExploring()
    {
        SetState(GameState.Exploring);
    }
}
