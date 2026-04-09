using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject mainPlayerCam; // Your FPS Camera
    public GameObject winCam;        // The one called "win cam"
    public GameObject loseCam;       // The one called "lose cam"

    [Header("References")]
    public HouseSinkerController house;

    public void LoseGame()
    {
        // Disable player control/view
        mainPlayerCam.SetActive(false);

        // Show the house sinking from the "lose cam" spot
        loseCam.SetActive(true);

        house.StartSinking();
        Debug.Log("Game Over: The house sinks with you inside.");
    }

    public void WinGame()
    {
        // Disable player view
        mainPlayerCam.SetActive(false);

        // Show the cinematic "win cam" view
        winCam.SetActive(true);

        house.StartSinking();
        Debug.Log("Victory: You escaped before the mansion vanished.");
    }
}

// attach to ending manager