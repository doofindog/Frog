using System;
using UnityEngine;
using UnityEngine.Events;

public class WinLoseEvaluator : MonoBehaviour
{
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private DropZoneGrid grid;

    [Space]
    public UnityEvent onWin;
    public UnityEvent onLose;

    private void OnEnable()  => DragManager.OnFrogPlaced += HandleFrogPlaced;
    private void OnDisable() => DragManager.OnFrogPlaced -= HandleFrogPlaced;

    private void HandleFrogPlaced()
    {
        if (queue.Count > 0) return;

        grid ??= FindAnyObjectByType<DropZoneGrid>();

        bool allSatisfied = EvaluateWinConditions(out string failReason);

        if (allSatisfied)
        {
            Debug.Log("[WinLose] Win");
            onWin.Invoke();
            if (levelLoader.HasNextLevel)
                levelLoader.LoadNextLevel();
        }
        else
        {
            Debug.Log($"[WinLose] Lose — {failReason}");
            onLose.Invoke();
        }
    }

    private bool EvaluateWinConditions(out string failReason)
    {
        var level = levelLoader.CurrentLevel;
        if (level == null || level.winConditions == null)
        {
            failReason = null;
            return true;
        }

        foreach (var condition in level.winConditions)
        {
            if (condition.verb == RuleVerb.Likes) continue;

            if (condition.objectType == RuleObjectType.Frog)
            {
                var posA = grid.FindFrog(condition.subjectFrogName);
                var posB = grid.FindFrog(condition.objectName);

                if (posA == null || posB == null)
                {
                    failReason = $"Frog not found: {condition.subjectFrogName} or {condition.objectName}";
                    Debug.LogWarning($"[WinLose] {failReason} — grid has: [{grid.GetOccupantNames()}]");
                    return false;
                }

                bool adjacent = (posA.Value - posB.Value).sqrMagnitude == 1;
                bool satisfied = condition.verb == RuleVerb.Loves ? adjacent : !adjacent;
                if (!satisfied)
                {
                    failReason = $"{condition.subjectFrogName} {condition.verb} {condition.objectName}";
                    return false;
                }
            }
            else
            {
                if (!Enum.TryParse<TileType>(condition.objectName, true, out TileType target))
                {
                    Debug.LogWarning($"[WinLose] Unknown tile '{condition.objectName}' for {condition.subjectFrogName}");
                    continue;
                }

                Vector2Int? coord = grid.FindFrog(condition.subjectFrogName);
                if (coord == null)
                {
                    failReason = $"Frog not found: {condition.subjectFrogName}";
                    Debug.LogWarning($"[WinLose] {failReason} — grid has: [{grid.GetOccupantNames()}]");
                    return false;
                }

                TileType actual = grid.GetZone(coord.Value).tileType;
                bool satisfied = condition.verb == RuleVerb.Loves ? actual == target : actual != target;
                if (!satisfied)
                {
                    failReason = $"{condition.subjectFrogName} {condition.verb} {target} (on {actual})";
                    return false;
                }
            }
        }

        failReason = null;
        return true;
    }
}
