using UnityEngine; // This was missing
using UnityEngine.UI;
using System.Collections.Generic;

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
            if (Input.GetKeyDown(KeyCode.RightArrow)) ChangePage(1);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangePage(-1);
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

    public void SelectPageDirectly()
    {
        if (puzzleSolved) return;
        Sprite currentSprite = allBookPages[currentPageIndex];

        if (!selectedSprites.Contains(currentSprite))
        {
            selectedSprites.Add(currentSprite);
            Debug.Log($"Picked page {currentPageIndex}. total: {selectedSprites.Count}/3");
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

        if (matches == 3) SpawnKey();
        else
        {
            Debug.Log("Wrong images! Resetting...");
            selectedSprites.Clear();
        }
    }

    void SpawnKey()
    {
        puzzleSolved = true;
        if (keyPrefab != null && keySpawnPoint != null)
            Instantiate(keyPrefab, keySpawnPoint.position, keySpawnPoint.rotation);

        bookUI.SetActive(false);
    }
} // This bracket MUST be here