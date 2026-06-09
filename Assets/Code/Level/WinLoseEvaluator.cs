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
                bool adjacent = grid.AreFrogsAdjacent(rule.subjectFrogName, rule.objectName);
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
                if (coord == null) continue;

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
