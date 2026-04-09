using UnityEngine;

public class TumblerInteractable : MonoBehaviour, IInteractable
{
    [Header("References")]
    public TumblerLock parentLock;
    public int tumblerIndex;

    public string GetPromptText() => "Rotate tumbler [E]";

    public bool CanInteract() => parentLock != null && !parentLock.IsSolved();

    public void Interact()
    {
        parentLock?.RotateTumbler(tumblerIndex);
    }
}
