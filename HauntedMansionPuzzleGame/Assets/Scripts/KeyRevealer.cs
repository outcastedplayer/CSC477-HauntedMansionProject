using UnityEngine;

public class KeyRevealer : MonoBehaviour
{
    [Header("The key object in the scene (starts disabled)")]
    public GameObject keyObject;

    // Call this from OfficePuzzle when the puzzle is solved
    // Either call it directly in code: GetComponent<KeyRevealer>().RevealKey()
    // Or wire it to a UnityEvent / Button OnClick in the Inspector
    public void RevealKey()
    {
        if (keyObject)
        {
            keyObject.SetActive(true);
            SoundManager.Instance?.PlaySFX("puzzle_solve");
            Debug.Log("[KeyRevealer] Key revealed: " + keyObject.name);
        }
    }
}
