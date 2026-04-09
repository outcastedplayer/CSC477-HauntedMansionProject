using UnityEngine;

public class CandleInteract : MonoBehaviour
{
    public GameObject flameObject;
    public CandlePuzzleManager puzzleManager;

    void OnMouseDown()
    {
        // 1. This prints the exact moment Unity detects a click on this object's collider
        Debug.Log("SUCCESS: Mouse clicked on " + gameObject.name);

        if (flameObject.activeSelf)
        {
            // 2. This prints if the script successfully talks to the Manager
            Debug.Log("The flame is active! Sending click to the Puzzle Manager...");
            puzzleManager.OnCandleClicked(this);
        }
        else
        {
            // 3. This prints if you clicked it, but the flame was already off
            Debug.Log("Clicked, but the flame is already off!");
        }
    }

    public void TurnOff()
    {
        flameObject.SetActive(false);
    }

    public void TurnOn()
    {
        flameObject.SetActive(true);
    }
}