using UnityEngine;

public class KeypadButtonInteractable : MonoBehaviour, IInteractable
{
    [Header("References")]
    public KeypadInput parentKeypad;

    [Header("Button Settings")]
    [Tooltip("Set to 1-9 for number buttons, or -1 for the reset button")]
    public int digit = 1;

    [Header("Press Animation")]
    public float pressDepth = 0.05f;
    public float pressDuration = 0.1f;

    bool isAnimating;

    public string GetPromptText()
    {
        if (digit < 0) return "Reset [E]";
        return "Press " + digit + " [E]";
    }

    public bool CanInteract()
    {
        return parentKeypad != null && !parentKeypad.IsSolved() && !isAnimating;
    }

    public void Interact()
    {
        if (parentKeypad == null) return;

        if (digit < 0)
            parentKeypad.PressReset();
        else
            parentKeypad.PressDigit(digit);

        StartCoroutine(AnimatePress());
    }

    System.Collections.IEnumerator AnimatePress()
    {
        isAnimating = true;
        Vector3 original = transform.localPosition;
        Vector3 pressed = original - transform.forward * pressDepth;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressDuration;
            transform.localPosition = Vector3.Lerp(original, pressed, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressDuration;
            transform.localPosition = Vector3.Lerp(pressed, original, t);
            yield return null;
        }

        transform.localPosition = original;
        isAnimating = false;
    }
}
