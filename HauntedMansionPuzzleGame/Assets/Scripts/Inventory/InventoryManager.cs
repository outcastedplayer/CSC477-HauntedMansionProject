using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("UI References")]
    public Image[] slots;
    public Sprite emptySlotSprite;
    public GameObject inventoryPanel;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Tab;
    public Color selectedTint = new Color(1f, 1f, 0.5f, 1f);
    public Color defaultTint = Color.white;

    public InventoryState CurrentState { get; private set; } = InventoryState.Hidden;

    List<InventoryItem> items = new List<InventoryItem>();
    int selectedIndex = -1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (inventoryPanel) inventoryPanel.SetActive(true);
        RefreshUI();
    }

    void Update()
    {
        if (GameManager.Instance && GameManager.Instance.CurrentState == GameState.Escaped) return;
        if (GameManager.Instance && GameManager.Instance.CurrentState == GameState.Trapped) return;

        if (Input.GetKeyDown(toggleKey))
            ToggleVisibility();

        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
                break;
            }
        }
    }

    public bool AddItem(InventoryItem item)
    {
        if (items.Count >= slots.Length) return false;
        if (items.Exists(i => i.itemID == item.itemID)) return false;

        items.Add(item);
        RefreshUI();
        SoundManager.Instance?.PlaySFX("pickup");
        return true;
    }

    public bool RemoveItem(string itemID)
    {
        int index = items.FindIndex(i => i.itemID == itemID);
        if (index < 0) return false;

        items.RemoveAt(index);
        if (selectedIndex >= items.Count) selectedIndex = -1;
        RefreshUI();
        return true;
    }

    public bool HasItem(string itemID)
    {
        return items.Exists(i => i.itemID == itemID);
    }

    public InventoryItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
        return items[selectedIndex];
    }

    public bool UseSelectedItem()
    {
        InventoryItem selected = GetSelectedItem();
        if (selected == null) return false;

        if (selected.isConsumable)
        {
            RemoveItem(selected.itemID);
            selectedIndex = -1;
            CurrentState = InventoryState.Visible;
        }
        return true;
    }

    void SelectSlot(int index)
    {
        if (index >= items.Count)
        {
            selectedIndex = -1;
            CurrentState = InventoryState.Visible;
        }
        else if (selectedIndex == index)
        {
            selectedIndex = -1;
            CurrentState = InventoryState.Visible;
        }
        else
        {
            selectedIndex = index;
            CurrentState = InventoryState.ItemSelected;
        }
        RefreshUI();
    }

    void ToggleVisibility()
    {
        SoundManager.Instance?.PlaySFX("inventory_open");
    }

    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            if (i < items.Count && items[i] != null)
            {
                slots[i].sprite = items[i].icon;
                slots[i].color = (i == selectedIndex) ? selectedTint : defaultTint;
            }
            else
            {
                if (emptySlotSprite) slots[i].sprite = emptySlotSprite;
                slots[i].color = defaultTint;
            }
        }
    }
}
