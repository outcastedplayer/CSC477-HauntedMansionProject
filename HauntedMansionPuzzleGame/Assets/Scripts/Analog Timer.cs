using UnityEngine;
using UnityEngine.SceneManagement; // For reloading the game

public class AnalogTimer : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform minuteHand; // Represents Seconds irl
    public Transform hourHand;   // Represents Minutes irl

    [Header("Settings")]
    public float timeRemaining = 300f; // 5 minutes in seconds
    private float totalTime = 300f;
    private bool gameOver = false;

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

        // 1. Minute Hand (Seconds IRL)
        // Spins 360 degrees every 60 seconds
        float minuteRotation = (elapsedSeconds % 60f) / 60f * -360f;
        minuteHand.localRotation = Quaternion.Euler(0, 0, minuteRotation);

        // 2. Hour Hand (Minutes IRL)
        // Starts at 7 o'clock (-210 degrees)
        // Over 5 minutes, it needs to travel 150 degrees to reach 12 o'clock
        float startAngle = -210f;
        float degreesToTravel = 150f; // The gap between 7 and 12

        float hourRotation = startAngle - ((elapsedSeconds / totalTime) * degreesToTravel);
        hourHand.localRotation = Quaternion.Euler(0, 0, hourRotation);
    }

    void GameOver()
    {
        gameOver = true;
        Debug.Log("<color=red>TIME IS UP! GAME OVER.</color>");

        // Example: Reload the scene or show a Game Over screen
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}