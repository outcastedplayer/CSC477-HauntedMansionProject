using UnityEngine;

public class CandlePuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("Drag your candles here in the EXACT order the player needs to click them.")]
    public CandleInteract[] correctOrder;
    private int currentStep = 0;

    [Header("Win Reward")]
    public GameObject keyObject;

    public void OnCandleClicked(CandleInteract clickedCandle)
    {
        // 1. Immediately turn off the clicked candle so the player gets visual feedback
        clickedCandle.TurnOff();

        // 2. Was it the correct choice?
        if (clickedCandle == correctOrder[currentStep])
        {
            currentStep++;

            // Did we finish the puzzle?
            if (currentStep >= correctOrder.Length)
            {
                WinPuzzle();
            }
        }
        else
        {
            // 3. Wrong choice! Wait half a second, then reignite all of them
            currentStep = 0;
            Invoke("ResetPuzzle", 0.5f); // 0.5f is the delay in seconds
        }
    }

    void ResetPuzzle()
    {
        // Turn all the flames back on
        foreach (CandleInteract candle in correctOrder)
        {
            candle.TurnOn();
        }
        Debug.Log("Wrong order! All candles reignited.");
    }

    void WinPuzzle()
    {
        Debug.Log("Puzzle Solved!");
        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
    }
}