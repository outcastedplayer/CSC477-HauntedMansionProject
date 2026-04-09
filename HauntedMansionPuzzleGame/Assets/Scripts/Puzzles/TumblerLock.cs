using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TumblerLock : MonoBehaviour
{
    [Header("Tumblers")]
    public TumblerData[] tumblers;
    public float rotateSpeed = 8f;
    public float rotationPerFace = 60f;
    [Tooltip("Which local axis the tumblers spin around")]
    public RotationAxis tumblerAxis = RotationAxis.X;

    public enum RotationAxis { X, Y, Z }

    [Header("Lever")]
    public Transform lever;
    public float leverPullAngle = 45f;
    public float leverSpeed = 4f;

    [Header("Morse Code Light")]
    public Light morseLight;
    public Renderer morseLightRenderer;
    public Material lightOnMat;
    public Material lightOffMat;
    public float dotDuration = 0.2f;
    public float dashDuration = 0.6f;
    public float symbolGap = 0.2f;
    public float letterGap = 0.6f;

    [Header("Events")]
    public UnityEvent OnTumblerSolved;

    bool isSolved;
    bool isBlinking;
    int[] currentFaceIndex;
    Quaternion[] initialRotation;

    // Morse code for H-E-L-P
    static readonly string[] morseLetters = { "....", ".", ".-..", ".--." };

    [System.Serializable]
    public struct TumblerData
    {
        public Transform transform;
        public int correctFaceIndex;
        [Tooltip("The 6 letters on this tumbler's faces, e.g. HARMTW")]
        public string faceLetters;
    }

    void Start()
    {
        currentFaceIndex = new int[tumblers.Length];
        initialRotation = new Quaternion[tumblers.Length];
        for (int i = 0; i < tumblers.Length; i++)
        {
            currentFaceIndex[i] = 0;
            initialRotation[i] = tumblers[i].transform.localRotation;
        }

        SetMorseLight(false);
    }

    // ── Called by TumblerInteractable on each tumbler ────

    public void RotateTumbler(int index)
    {
        if (isSolved || index < 0 || index >= tumblers.Length) return;

        currentFaceIndex[index] = (currentFaceIndex[index] + 1) % 6;
        SoundManager.Instance?.PlaySFX("tumbler_click");
        StartCoroutine(AnimateTumbler(index));

        if (CheckAllTumblers())
        {
            isSolved = true;
            SoundManager.Instance?.PlaySFX("tumbler_solved");
            OnTumblerSolved?.Invoke();
        }
    }

    IEnumerator AnimateTumbler(int index)
    {
        Transform t = tumblers[index].transform;
        Quaternion start = t.localRotation;
        float angle = currentFaceIndex[index] * rotationPerFace;
        Quaternion faceRotation;
        switch (tumblerAxis)
        {
            case RotationAxis.Y: faceRotation = Quaternion.Euler(0, angle, 0); break;
            case RotationAxis.Z: faceRotation = Quaternion.Euler(0, 0, angle); break;
            default: faceRotation = Quaternion.Euler(angle, 0, 0); break;
        }
        // Apply on top of the tumbler's original scene rotation
        Quaternion target = initialRotation[index] * faceRotation;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * rotateSpeed;
            t.localRotation = Quaternion.Slerp(start, target, Mathf.SmoothStep(0, 1, elapsed));
            yield return null;
        }
        t.localRotation = target;
    }

    bool CheckAllTumblers()
    {
        for (int i = 0; i < tumblers.Length; i++)
        {
            if (currentFaceIndex[i] != tumblers[i].correctFaceIndex)
                return false;
        }
        return true;
    }

    // ── Called by LeverInteractable ──────────────────────

    public void PullLever()
    {
        if (isSolved || isBlinking) return;
        StartCoroutine(LeverSequence());
    }

    IEnumerator LeverSequence()
    {
        isBlinking = true;
        SoundManager.Instance?.PlaySFX("lever_pull");

        // Animate lever down
        if (lever)
        {
            Quaternion start = lever.localRotation;
            Quaternion target = start * Quaternion.Euler(leverPullAngle, 0, 0);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * leverSpeed;
                lever.localRotation = Quaternion.Slerp(start, target, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(PlayMorseSequence());

            // Animate lever back up
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * leverSpeed;
                lever.localRotation = Quaternion.Slerp(target, start, t);
                yield return null;
            }
            lever.localRotation = start;
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(PlayMorseSequence());
        }

        isBlinking = false;
    }

    IEnumerator PlayMorseSequence()
    {
        foreach (string letter in morseLetters)
        {
            foreach (char symbol in letter)
            {
                SetMorseLight(true);
                SoundManager.Instance?.PlaySFX("morse_beep");

                float duration = (symbol == '.') ? dotDuration : dashDuration;
                yield return new WaitForSeconds(duration);

                SetMorseLight(false);
                yield return new WaitForSeconds(symbolGap);
            }
            yield return new WaitForSeconds(letterGap);
        }
    }

    void SetMorseLight(bool on)
    {
        if (morseLight)
            morseLight.enabled = on;

        if (morseLightRenderer)
            morseLightRenderer.material = on ? lightOnMat : lightOffMat;
    }

    public bool IsSolved() => isSolved;
}
