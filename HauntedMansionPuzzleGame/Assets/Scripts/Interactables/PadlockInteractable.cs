using UnityEngine;
using UnityEngine.Events;

public class PadlockInteractable : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string requiredKeyID;
    public string padlockName = "Padlock";

    [Header("References")]
    public FrontDoorController doorController;

    [Header("Events")]
    public UnityEvent OnUnlocked;

    bool isUnlocked;

    public string GetPromptText()
    {
        if (isUnlocked) return "";

        var inventory = InventoryManager.Instance;
        if (inventory == null) return $"Locked — needs a key";

        if (!inventory.HasItem(requiredKeyID))
            return $"This padlock needs a key...";

        var selected = inventory.GetSelectedItem();
        if (selected != null && selected.itemID == requiredKeyID)
            return $"Use {selected.itemName} [E]";

        return "Select the correct key first";
    }

    public bool CanInteract()
    {
        return !isUnlocked;
    }

    public void Interact()
    {
        Debug.Log("Tried interacting with " + padlockName);

        if (isUnlocked)
        {
            Debug.Log("Already unlocked");
            return;
        }

        var inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.Log("No InventoryManager");
            return;
        }

        var selected = inventory.GetSelectedItem();
        if (selected == null)
        {
            Debug.Log("No selected item");
            return;
        }

        Debug.Log("Selected item: " + selected.itemID + " Required: " + requiredKeyID);

        if (selected.itemID != requiredKeyID)
        {
            Debug.Log("Wrong key selected");
            // SoundManager.Instance?.PlaySFX("locked_rattle");
            return;
        }

        Debug.Log("Correct key used, unlocking");
        // SoundManager.Instance?.PlaySFX("key_insert");
        inventory.UseSelectedItem();
        isUnlocked = true;

        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;

        Debug.Log("Calling door controller");
        OnUnlocked?.Invoke();
        if (doorController == null)
        {
            Debug.LogError("Padlock " + gameObject.name + " has no doorController assigned!");
        }
        else
        {
            Debug.Log("Sending unlock to " + doorController.name);
            doorController.OnPadlockUnlocked();
        }
    }
}
