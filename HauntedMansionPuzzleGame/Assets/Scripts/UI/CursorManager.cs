using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Sprites")]
    public Sprite pointCursor;
    public Sprite grabCursor;

    Image cursorImage;
    RectTransform rectTransform;

    void Awake()
    {
        cursorImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.position = Input.mousePosition;
    }

    public void SetPointer()
    {
        if (cursorImage && pointCursor)
            cursorImage.sprite = pointCursor;
    }

    public void SetGrab()
    {
        if (cursorImage && grabCursor)
            cursorImage.sprite = grabCursor;
    }
}
