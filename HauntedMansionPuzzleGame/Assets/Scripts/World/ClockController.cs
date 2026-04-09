using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ClockController : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform minuteHand;
    public Transform hourHand;

    [Header("Timer Settings")]
    public float gameDuration = 600f;
    public float warningThreshold = 120f;

    [Header("Tick Sounds")]
    public float normalTickInterval = 1f;
    public float fastTickInterval = 0.4f;

    [Header("Chime Thresholds (seconds remaining)")]
    public float[] chimeAt = { 300f, 120f, 60f };

    [Header("Events")]
    public UnityEvent OnTimeExpired;
    public UnityEvent OnWarningStarted;

    public ClockState CurrentState { get; private set; } = ClockState.Running;
    public float TimeRemaining => Mathf.Max(0f, gameDuration - elapsedTime);

    float elapsedTime;
    float tickTimer;
    int nextChimeIndex;

    void Start()
    {
        elapsedTime = 0f;
        nextChimeIndex = 0;
        CurrentState = ClockState.Running;
    }

    void Update()
    {
        if (CurrentState == ClockState.Stopped || CurrentState == ClockState.Expired) return;
        if (GameManager.Instance && GameManager.Instance.IsPaused) return;

        elapsedTime += Time.deltaTime;
        float remaining = TimeRemaining;

        // Update clock hands
        UpdateClockHands();

        // Check chime thresholds
        CheckChimes(remaining);

        // Check warning state
        if (CurrentState == ClockState.Running && remaining <= warningThreshold)
        {
            CurrentState = ClockState.Warning;
            OnWarningStarted?.Invoke();
            SoundManager.Instance?.CrossfadeMusic(SoundManager.Instance.urgencyMusic);
        }

        // Tick sound
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            SoundManager.Instance?.PlaySFX("clock_tick");
            tickTimer = (CurrentState == ClockState.Warning) ? fastTickInterval : normalTickInterval;
        }

        // Time expired
        if (remaining <= 0f)
        {
            CurrentState = ClockState.Expired;
            SoundManager.Instance?.PlaySFX("clock_chime");
            OnTimeExpired?.Invoke();
            GameManager.Instance?.OnTimeExpired();
        }
    }

    void UpdateClockHands()
    {
        float progress = elapsedTime / gameDuration;

        // Minute hand: full 360° sweep over the game duration
        if (minuteHand)
            minuteHand.localRotation = Quaternion.Euler(0, 0, -progress * 360f);

        // Hour hand: 1/12 speed of minute hand
        if (hourHand)
            hourHand.localRotation = Quaternion.Euler(0, 0, -progress * 30f);
    }

    void CheckChimes(float remaining)
    {
        if (nextChimeIndex >= chimeAt.Length) return;

        if (remaining <= chimeAt[nextChimeIndex])
        {
            SoundManager.Instance?.PlaySFX("clock_chime");
            nextChimeIndex++;
        }
    }

    public void StopClock()
    {
        CurrentState = ClockState.Stopped;
    }

    public void PauseClock()
    {
        // Handled by checking GameManager.IsPaused in Update
    }

    public float GetElapsedTime() => elapsedTime;
    public float GetProgress() => Mathf.Clamp01(elapsedTime / gameDuration);
}
