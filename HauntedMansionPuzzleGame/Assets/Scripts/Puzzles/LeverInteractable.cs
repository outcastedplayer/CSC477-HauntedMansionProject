using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [Header("References")]
    public TumblerLock parentLock;

    public string GetPromptText() => "Pull lever [E]";

    public bool CanInteract() => parentLock != null && !parentLock.IsSolved();

    public void Interact()
    {
        parentLock?.PullLever();
    }
}
