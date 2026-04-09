using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FrontDoorController : MonoBehaviour
{
    [Header("Door")]
    public Transform door;

    [Header("Padlocks")]
    public PadlockInteractable[] padlocks;

    [Header("Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Events")]
    public UnityEvent OnAllPadlocksUnlocked;
    public UnityEvent OnDoorFullyOpen;

    public DoorState CurrentState { get; private set; } = DoorState.FullyLocked;

    int unlockedCount;

    public void OnPadlockUnlocked()
    {
        Debug.Log("Padlock unlocked. Count = " + unlockedCount);
        unlockedCount++;
        GameManager.Instance?.OnPadlockUnlocked();
        Debug.Log("Padlock unlocked. Count = " + unlockedCount);
        if (unlockedCount >= padlocks.Length)
        {
            CurrentState = DoorState.Unlocked;
            OnAllPadlocksUnlocked?.Invoke();
            StartCoroutine(OpenDoorSequence());
        }
        else
        {
            CurrentState = DoorState.PartiallyLocked;
        }
        Debug.Log("Padlock unlocked. Count = " + unlockedCount);
    }

    IEnumerator OpenDoorSequence()
    {
        Debug.Log("Opening door...");
        CurrentState = DoorState.Opening;
        SoundManager.Instance?.PlaySFX("door_open");

        if (door)
        {
            Quaternion start = door.localRotation;
            Quaternion target = start * Quaternion.Euler(0, openAngle, 0);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                float smooth = Mathf.SmoothStep(0f, 1f, t);
                door.localRotation = Quaternion.Slerp(start, target, smooth);
                yield return null;
            }
        }

        CurrentState = DoorState.Open;
        OnDoorFullyOpen?.Invoke();
    }

    public int GetRemainingPadlocks()
    {
        return padlocks.Length - unlockedCount;
    }
}
