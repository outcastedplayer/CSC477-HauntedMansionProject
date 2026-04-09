using UnityEngine;
using UnityEngine.InputSystem;

public class BookPhysicalInteract : MonoBehaviour
{
    public OfficePuzzle puzzleManager;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenTheBook();
            }
        }
    }

    // THIS IS THE NEW PART
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseTheBook();
        }
    }

    public void OpenTheBook()
    {
        puzzleManager.bookUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTheBook()
    {
        puzzleManager.bookUI.SetActive(false);
        // Re-lock the cursor so the player can look around again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}