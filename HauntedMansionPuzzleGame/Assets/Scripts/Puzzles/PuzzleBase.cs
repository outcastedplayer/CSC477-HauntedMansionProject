using UnityEngine;
using UnityEngine.Events;

public abstract class PuzzleBase : MonoBehaviour
{
    [Header("Puzzle Base")]
    public InventoryItem keyReward;
    public Transform keySpawnPoint;
    public bool giveKeyDirectly = true;

    [Header("Events")]
    public UnityEvent OnPuzzleSolved;

    public PuzzleState CurrentState { get; protected set; } = PuzzleState.Inactive;

    public virtual void ActivatePuzzle()
    {
        if (CurrentState == PuzzleState.Solved) return;
        CurrentState = PuzzleState.Active;
        OnPuzzleActivated();
    }

    public void SetInProgress()
    {
        if (CurrentState == PuzzleState.Solved) return;
        CurrentState = PuzzleState.InProgress;
    }

    public void CompletePuzzle()
    {
        if (CurrentState == PuzzleState.Solved) return;
        CurrentState = PuzzleState.Solved;

        SoundManager.Instance?.PlaySFX("puzzle_solve");
        OnPuzzleSolved?.Invoke();
        OnPuzzleCompleted();

        if (giveKeyDirectly && keyReward != null)
        {
            InventoryManager.Instance?.AddItem(keyReward);
            GameManager.Instance?.OnKeyCollected();
        }
    }

    public void FailPuzzle()
    {
        CurrentState = PuzzleState.Failed;
        SoundManager.Instance?.PlaySFX("puzzle_fail");
        OnPuzzleFailed();
    }

    public virtual void ResetPuzzle()
    {
        if (CurrentState == PuzzleState.Solved) return;
        CurrentState = PuzzleState.Active;
        OnPuzzleReset();
    }

    protected abstract void OnPuzzleActivated();
    protected abstract bool CheckSolution();
    protected abstract void OnPuzzleCompleted();

    protected virtual void OnPuzzleFailed() { }
    protected virtual void OnPuzzleReset() { }
}
