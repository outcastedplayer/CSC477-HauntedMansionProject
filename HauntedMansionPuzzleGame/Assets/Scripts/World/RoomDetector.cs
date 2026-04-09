using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    public RoomType roomType;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance)
            GameManager.Instance.CurrentRoom = roomType;

        SoundManager.Instance?.SetAmbience(roomType);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance && GameManager.Instance.CurrentRoom == roomType)
        {
            GameManager.Instance.CurrentRoom = RoomType.Lobby;
            SoundManager.Instance?.SetAmbience(RoomType.Lobby);
        }
    }
}
