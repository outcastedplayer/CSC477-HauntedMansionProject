using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ParlorPuzzleBox : PuzzleBase
{
    public enum BoxState { Locked, PartiallyUnlocked, Opening, Solved }

    [Header("Sub-Puzzles")]
    public TumblerLock tumblerLock;
    public KeypadInput keypadInput;

    [Header("Indicator Lights")]
    public Renderer tumblerLight;
    public Renderer keypadLight;
    public Material lightRedMat;
    public Material lightGreenMat;

    [Header("Top Lid")]
    public Transform topLid;
    public float lidOpenAngle = 90f;
    public float openSpeed = 2f;

    [Header("Parlor Key")]
    public GameObject parlorKeyObject;

    [Header("Events")]
    public UnityEvent OnLidOpened;

    public BoxState CurrentBoxState { get; private set; } = BoxState.Locked;

    bool tumblerSolved;
    bool keypadSolved;

    void Start()
    {
        if (tumblerLight) tumblerLight.material = lightRedMat;
        if (keypadLight) keypadLight.material = lightRedMat;
        if (parlorKeyObject) parlorKeyObject.SetActive(false);

        if (tumblerLock) tumblerLock.OnTumblerSolved.AddListener(OnTumblerSolved);
        if (keypadInput) keypadInput.OnKeypadSolved.AddListener(OnKeypadSolved);
    }

    void OnTumblerSolved()
    {
        tumblerSolved = true;
        if (tumblerLight) tumblerLight.material = lightGreenMat;
        SoundManager.Instance?.PlaySFX("puzzle_solve");
        CheckBothSolved();
    }

    void OnKeypadSolved()
    {
        keypadSolved = true;
        if (keypadLight) keypadLight.material = lightGreenMat;
        SoundManager.Instance?.PlaySFX("puzzle_solve");
        CheckBothSolved();
    }

    void CheckBothSolved()
    {
        if (tumblerSolved && keypadSolved)
        {
            CurrentBoxState = BoxState.Opening;
            StartCoroutine(OpenLid());
        }
        else
        {
            CurrentBoxState = BoxState.PartiallyUnlocked;
        }
    }

    IEnumerator OpenLid()
    {
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance?.PlaySFX("box_open");

        if (topLid)
        {
            Quaternion start = topLid.localRotation;
            Quaternion target = start * Quaternion.Euler(-lidOpenAngle, 0, 0);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                topLid.localRotation = Quaternion.Slerp(start, target, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }
        }

        CurrentBoxState = BoxState.Solved;
        if (parlorKeyObject) parlorKeyObject.SetActive(true);
        OnLidOpened?.Invoke();

        CurrentState = PuzzleState.Solved;
        OnPuzzleSolved?.Invoke();
    }

    protected override void OnPuzzleActivated() { }
    protected override bool CheckSolution() => tumblerSolved && keypadSolved;
    protected override void OnPuzzleCompleted() { }
}