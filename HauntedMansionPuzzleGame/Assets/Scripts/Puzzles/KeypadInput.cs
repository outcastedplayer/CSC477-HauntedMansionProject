using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadInput : MonoBehaviour
{
    [Header("Solution")]
    public int[] correctCode;

    [Header("Events")]
    public UnityEvent OnKeypadSolved;

    List<int> enteredDigits = new List<int>();
    bool isSolved;

    // ── Called directly by KeypadButtonInteractable ──────

    public void PressDigit(int digit)
    {
        if (isSolved) return;

        enteredDigits.Add(digit);
        SoundManager.Instance?.PlaySFX("keypad_beep");
        Debug.Log("Keypad digit pressed: " + digit + " | Entered so far: " + enteredDigits.Count + "/" + correctCode.Length);

        if (enteredDigits.Count >= correctCode.Length)
        {
            if (CheckCode())
            {
                isSolved = true;
                Debug.Log("Keypad SOLVED!");
                SoundManager.Instance?.PlaySFX("puzzle_solve");
                OnKeypadSolved?.Invoke();
            }
            else
            {
                Debug.Log("Keypad WRONG code, resetting...");
                SoundManager.Instance?.PlaySFX("keypad_deny");
                StartCoroutine(ResetAfterDelay(0.5f));
            }
        }
    }

    public void PressReset()
    {
        if (isSolved) return;

        enteredDigits.Clear();
        SoundManager.Instance?.PlaySFX("ui_click");
        Debug.Log("Keypad reset");
    }

    bool CheckCode()
    {
        if (enteredDigits.Count != correctCode.Length) return false;
        for (int i = 0; i < correctCode.Length; i++)
        {
            if (enteredDigits[i] != correctCode[i])
                return false;
        }
        return true;
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enteredDigits.Clear();
    }

    public bool IsSolved() => isSolved;
}