using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinLoseEvaluator : MonoBehaviour
{
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private RuleBook ruleBook;
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

        bool allSatisfied = EvaluateRules(out string failReason);

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

    private bool EvaluateRules(out string failReason)
    {
        foreach (var display in ruleBook.GetDisplays())
        {
            RuleData rule = display.Data;

            if (rule.trueVerb == RuleVerb.Likes) continue;

            if (rule.objectType == RuleObjectType.Frog)
            {
                var posA = grid.FindFrog(rule.subjectFrogName);
                var posB = grid.FindFrog(rule.objectName);

                if (posA == null || posB == null)
                {
                    string found = grid.GetOccupantNames();
                    Debug.LogWarning($"[WinLose] Frog not found on '{grid.name}' — looking for '{rule.subjectFrogName}' and '{rule.objectName}', grid has: [{found}]");
                    failReason = $"Frog not found: {rule.subjectFrogName} or {rule.objectName}";
                    return false;
                }

                bool adjacent = (posA.Value - posB.Value).sqrMagnitude == 1;
                bool satisfied = rule.trueVerb == RuleVerb.Loves ? adjacent : !adjacent;
                if (!satisfied)
                {
                    failReason = $"{rule.subjectFrogName} {rule.trueVerb} {rule.objectName}";
                    return false;
                }
            }
            else
            {
                if (!Enum.TryParse<TileType>(rule.objectName, true, out TileType target))
                {
                    Debug.LogWarning($"[WinLose] Unknown tile name '{rule.objectName}' in rule for {rule.subjectFrogName}");
                    continue;
                }

                Vector2Int? coord = grid.FindFrog(rule.subjectFrogName);
                if (coord == null)
                {
                    string found = grid.GetOccupantNames();
                    Debug.LogWarning($"[WinLose] Frog '{rule.subjectFrogName}' not found on '{grid.name}', grid has: [{found}]");
                    failReason = $"Frog not found: {rule.subjectFrogName}";
                    return false;
                }

                TileType actual = grid.GetZone(coord.Value).tileType;
                bool satisfied = rule.trueVerb == RuleVerb.Loves ? actual == target : actual != target;
                if (!satisfied)
                {
                    failReason = $"{rule.subjectFrogName} {rule.trueVerb} {target} (on {actual})";
                    return false;
                }
            }
        }

        failReason = null;
        return true;
    }
}
