using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WinLoseEvaluator : MonoBehaviour
{
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private DropZoneGrid grid;
    [SerializeField] private RuleBook ruleBook;

    public static event System.Action OnLevelCleared;

    [Space]
    public UnityEvent onWin;
    public UnityEvent onLose;

    private void OnEnable()
    {
        DragManager.OnFrogPlaced += HandleFrogPlaced;
        RuleBook.OnRulesChanged += HandleRulesChanged;
    }

    private void OnDisable()
    {
        DragManager.OnFrogPlaced -= HandleFrogPlaced;
        RuleBook.OnRulesChanged -= HandleRulesChanged;
    }

    private void HandleFrogPlaced() => CheckWinLose();
    private void HandleRulesChanged() => CheckWinLose();

    private void CheckWinLose()
    {
        if (queue.Count > 0) return;

        // DropZoneGrid is destroyed and re-instantiated every level, so a destroyed reference
        // must be re-resolved. Using ??= here would miss that: Unity's "destroyed but not
        // null" objects only read as null through the overloaded == operator, not through ??=.
        if (grid == null) grid = FindAnyObjectByType<DropZoneGrid>();
        if (ruleBook == null) ruleBook = FindAnyObjectByType<RuleBook>();

        bool allSatisfied = EvaluateWinConditions(out string failReason);

        if (allSatisfied)
        {
            Debug.Log("[WinLose] Win");
            OnLevelCleared?.Invoke();
            onWin.Invoke();
        }
        else
        {
            Debug.Log($"[WinLose] Lose — {failReason}");
            onLose.Invoke();
        }
    }

    // A subject name can match several customers at once (e.g. a species like "Pink Frogs").
    // Every matching subject individually must satisfy the constraint; for frog-frog rules,
    // an object name is satisfied as long as the subject is adjacent to any one matching object.
    private bool EvaluateWinConditions(out string failReason)
    {
        foreach (var constraint in ruleBook.GetAllConstraints())
        {
            var subjectCoords = grid.FindFrogs(constraint.subjectFrog).ToList();
            if (subjectCoords.Count == 0)
            {
                failReason = $"Frog not found: {constraint.subjectFrog}";
                Debug.LogWarning($"[WinLose] {failReason} — grid has: [{grid.GetOccupantNames()}]");
                return false;
            }

            if (constraint.objectType == RuleObjectType.Tile)
            {
                foreach (var coord in subjectCoords)
                {
                    TileType actual = grid.GetZone(coord).tileType;
                    if (!constraint.IsSatisfiedBy(actual))
                    {
                        failReason = $"{constraint.subjectFrog} {constraint.verb} {string.Join(" and ", constraint.objectNames)} (on {actual})";
                        return false;
                    }
                }
            }
            else
            {
                foreach (var coord in subjectCoords)
                {
                    bool satisfied = constraint.IsSatisfiedBy(name => grid.FindFrogs(name).Any(c => DropZoneGrid.AreAdjacent(coord, c)));
                    if (!satisfied)
                    {
                        failReason = $"{constraint.subjectFrog} {constraint.verb} {string.Join(" and ", constraint.objectNames)}";
                        Debug.LogWarning($"[WinLose] {failReason} — grid has: [{grid.GetOccupantNames()}]");
                        return false;
                    }
                }
            }
        }

        failReason = null;
        return true;
    }
}
