using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public InventoryItem item;

    [Header("Optional Effects")]
    public float bobSpeed = 1.5f;
    public float bobHeight = 0.15f;
    public float rotateSpeed = 45f;
    public bool animateIdle = false;

    bool pickedUp;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (pickedUp || !animateIdle) return;

        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + Vector3.up * yOffset;
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    public string GetPromptText()
    {
        return item != null ? $"Pick up {item.itemName} [E]" : "Pick up [E]";
    }

    public bool CanInteract()
    {
        return !pickedUp;
    }

    public void Interact()
    {
        if (pickedUp) return;

        if (InventoryManager.Instance && InventoryManager.Instance.AddItem(item))
        {
            pickedUp = true;

            if (item.isKeyItem)
                GameManager.Instance?.OnKeyCollected();

            gameObject.SetActive(false);
        }
    }
}
