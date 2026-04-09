using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera cutsceneCamera;
    public Camera playerCamera;

    [Header("Win Cutscene")]
    public Transform[] winCameraPath;
    public float winDuration = 7f;

    [Header("Lose Cutscene")]
    public Transform[] loseCameraPath;
    public float loseDuration = 7f;

    [Header("Lighting")]
    public Light directionalLight;
    public Light playerLight;

    [Header("Doors (for lose sequence)")]
    public Transform[] doorObjects;
    public float doorSlamAngle = 90f;

    [Header("Fade")]
    public CanvasGroup fadeOverlay;
    public float fadeDuration = 1.5f;

    [Header("Settings")]
    public bool allowSkip = true;

    [Header("Events")]
    public UnityEvent OnWinCutsceneFinished;
    public UnityEvent OnLoseCutsceneFinished;

    public CutsceneState CurrentState { get; private set; } = CutsceneState.Idle;

    Coroutine activeRoutine;

    void Start()
    {
        if (cutsceneCamera) cutsceneCamera.gameObject.SetActive(false);
        if (fadeOverlay) fadeOverlay.alpha = 0f;
    }

    void Update()
    {
        if (CurrentState == CutsceneState.Playing && allowSkip && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    // ── Public API ───────────────────────────────────────

    public void PlayWinCutscene()
    {
        if (CurrentState != CutsceneState.Idle) return;
        activeRoutine = StartCoroutine(WinSequence());
    }

    public void PlayLoseCutscene()
    {
        if (CurrentState != CutsceneState.Idle) return;
        activeRoutine = StartCoroutine(LoseSequence());
    }

    public void SkipCutscene()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
        FinishCutscene();
    }

    // ── Win Sequence ─────────────────────────────────────

    IEnumerator WinSequence()
    {
        CurrentState = CutsceneState.Playing;
        SoundManager.Instance?.StopMusic(0.5f);

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f, 0.5f));

        // Switch cameras
        SwitchToCutsceneCamera();

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f, 0.5f));

        SoundManager.Instance?.PlaySFX("door_open");

        // Dolly camera through the win path
        if (winCameraPath.Length > 1)
            yield return StartCoroutine(DollyCamera(winCameraPath, winDuration - 2f));

        // Brighten light at the end (sunlight through door)
        if (directionalLight)
            yield return StartCoroutine(LerpLightIntensity(directionalLight, directionalLight.intensity, 3f, 1f));

        // Play win music
        SoundManager.Instance?.PlayMusic(SoundManager.Instance.winMusic);

        // Fade to white
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        FinishCutscene();
        UIManager.Instance?.ShowWinScreen();
        OnWinCutsceneFinished?.Invoke();
    }

    // ── Lose Sequence ────────────────────────────────────

    IEnumerator LoseSequence()
    {
        CurrentState = CutsceneState.Playing;

        // Chime loudly
        SoundManager.Instance?.PlaySFX("clock_chime");
        yield return new WaitForSeconds(1f);

        // Slam doors shut one by one
        if (doorObjects != null)
        {
            foreach (var door in doorObjects)
            {
                if (door == null) continue;
                SoundManager.Instance?.PlaySFX("trap_slam");
                StartCoroutine(SlamDoor(door));
                yield return new WaitForSeconds(0.4f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Flicker and dim lights
        if (playerLight)
            StartCoroutine(FlickerLight(playerLight, 2f));

        if (directionalLight)
            StartCoroutine(LerpLightIntensity(directionalLight, directionalLight.intensity, 0f, 2f));

        // Play lose music
        SoundManager.Instance?.StopMusic(0.5f);
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance?.PlayMusic(SoundManager.Instance.loseMusic);

        yield return new WaitForSeconds(2f);

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        FinishCutscene();
        UIManager.Instance?.ShowLoseScreen();
        OnLoseCutsceneFinished?.Invoke();
    }

    // ── Camera Utilities ─────────────────────────────────

    void SwitchToCutsceneCamera()
    {
        if (playerCamera) playerCamera.gameObject.SetActive(false);
        if (cutsceneCamera)
        {
            cutsceneCamera.gameObject.SetActive(true);
            if (winCameraPath.Length > 0)
            {
                cutsceneCamera.transform.position = winCameraPath[0].position;
                cutsceneCamera.transform.rotation = winCameraPath[0].rotation;
            }
        }
    }

    IEnumerator DollyCamera(Transform[] path, float duration)
    {
        if (path.Length < 2 || !cutsceneCamera) yield break;

        float totalSegments = path.Length - 1;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float scaledProgress = progress * totalSegments;

            int segIndex = Mathf.Min(Mathf.FloorToInt(scaledProgress), (int)totalSegments - 1);
            float segT = scaledProgress - segIndex;
            float smooth = Mathf.SmoothStep(0f, 1f, segT);

            cutsceneCamera.transform.position = Vector3.Lerp(
                path[segIndex].position, path[segIndex + 1].position, smooth);
            cutsceneCamera.transform.rotation = Quaternion.Slerp(
                path[segIndex].rotation, path[segIndex + 1].rotation, smooth);

            yield return null;
        }
    }

    // ── Effect Utilities ─────────────────────────────────

    IEnumerator Fade(float from, float to, float duration)
    {
        if (!fadeOverlay) yield break;

        fadeOverlay.gameObject.SetActive(true);
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeOverlay.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        fadeOverlay.alpha = to;
    }

    IEnumerator LerpLightIntensity(Light light, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            light.intensity = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        light.intensity = to;
    }

    IEnumerator FlickerLight(Light light, float duration)
    {
        float original = light.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            light.intensity = Random.Range(0f, original);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            elapsed += 0.1f;
        }
        light.intensity = 0f;
    }

    IEnumerator SlamDoor(Transform door)
    {
        Quaternion start = door.localRotation;
        Quaternion target = start * Quaternion.Euler(0, doorSlamAngle, 0);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 6f; // fast slam
            door.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }
    }

    void FinishCutscene()
    {
        CurrentState = CutsceneState.Finished;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
