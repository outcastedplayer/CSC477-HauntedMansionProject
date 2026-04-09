using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalogTimer : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform minuteHand;
    public Transform hourHand;

    [Header("Settings")]
    public float timeRemaining = 300f;
    private float totalTime = 300f;
    private bool gameOver = false;

    [Header("Ending Logic")]
    public EndingManager endingManager; // Drag your EndingManager here in the Inspector

    void Update()
    {
        if (gameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateClockHands();
        }
        else
        {
            GameOver();
        }
    }

    void UpdateClockHands()
    {
        float elapsedSeconds = totalTime - timeRemaining;

        // 1. Minute Hand
        float minuteRotation = (elapsedSeconds % 60f) / 60f * -360f;
        minuteHand.localRotation = Quaternion.Euler(0, 0, minuteRotation);

        // 2. Hour Hand
        float startAngle = -210f; // 7 o'clock
        float degreesToTravel = 150f;

        // Moves clockwise toward -360
        float hourRotation = startAngle - ((elapsedSeconds / totalTime) * degreesToTravel);
        hourHand.localRotation = Quaternion.Euler(0, 0, hourRotation);
    }

    void GameOver()
    {
        gameOver = true;
        Debug.Log("<color=red>TIME IS UP! GAME OVER.</color>");

        // Trigger the sinking house and the "lose cam" switch
        if (endingManager != null)
        {
            endingManager.LoseGame();
        }
        else
        {
            // Fallback: Just reload if you forgot to assign the manager
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}