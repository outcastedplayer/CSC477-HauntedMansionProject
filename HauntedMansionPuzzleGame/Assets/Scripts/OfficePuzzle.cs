using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OfficePuzzle : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bookUI;
    public Image pageImageDisplay;

    [Header("Sprites")]
    public Sprite[] allBookPages;
    public Sprite[] doorHintSprites;

    [Header("Reward")]
    public GameObject keyPrefab;
    public Transform keySpawnPoint;

    private int currentPageIndex = 0;
    private List<Sprite> selectedSprites = new List<Sprite>();
    private bool puzzleSolved = false;

    void Start()
    {
        if (allBookPages.Length > 0 && pageImageDisplay != null)
            UpdatePageUI();
    }

    void Update()
    {
        if (bookUI != null && bookUI.activeSelf)
        {
            if (Keyboard.current.xKey.wasPressedThisFrame) ChangePage(1);
            if (Keyboard.current.zKey.wasPressedThisFrame) ChangePage(-1);
        }
    }

    void ChangePage(int direction)
    {
        currentPageIndex = Mathf.Clamp(currentPageIndex + direction, 0, allBookPages.Length - 1);
        UpdatePageUI();
    }

    void UpdatePageUI()
    {
        pageImageDisplay.sprite = allBookPages[currentPageIndex];
    }

    public void OpenBook()
    {
        bookUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SelectPageDirectly()
    {
        // 1. THIS IS THE GUARD: If the puzzle is already solved, stop right here!
        if (puzzleSolved)
        {
            Debug.Log("<color=white>Puzzle already solved. No more keys for you!</color>");
            return;
        }

        Sprite currentSprite = allBookPages[currentPageIndex];

        if (!selectedSprites.Contains(currentSprite))
        {
            selectedSprites.Add(currentSprite);
            Debug.Log($"<color=cyan>Selected Page {currentPageIndex + 1}</color>. Total: {selectedSprites.Count}/3");
        }
        else
        {
            Debug.Log("<color=yellow>You already selected this page!</color>");
        }

        if (selectedSprites.Count == 3)
        {
            CheckPuzzleResult();
        }
    }

    void CheckPuzzleResult()
    {
        int matches = 0;
        foreach (Sprite picked in selectedSprites)
        {
            foreach (Sprite hint in doorHintSprites)
            {
                if (picked == hint) { matches++; break; }
            }
        }

        // DEBUG: Shows you how many were correct before spawning or resetting
        Debug.Log($"<color=white>Checking Results...</color> Matches found: {matches}/3");

        if (matches == 3)
        {
            SpawnKey();
        }
        else
        {
            Debug.Log("<color=red>Wrong combination! Selection cleared. Try again.</color>");
            selectedSprites.Clear();
        }
    }

    void SpawnKey()
    {
        GetComponent<KeyRevealer>().RevealKey();
        puzzleSolved = true;
        Debug.Log("<color=green>Puzzle Solved! Spawning Key.</color>");
        bookUI.SetActive(false);
        // Lock cursor back for movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}