using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinLoseEvaluator : MonoBehaviour
{
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private RuleBook ruleBook;
    [SerializeField] private LevelLoader levelLoader;

    [Space]
    public UnityEvent onWin;
    public UnityEvent onLose;

    private void OnEnable()  => DragManager.OnFrogPlaced += HandleFrogPlaced;
    private void OnDisable() => DragManager.OnFrogPlaced -= HandleFrogPlaced;

    private void HandleFrogPlaced()
    {
        if (queue.Count > 0) return;

        var placement = BuildPlacementMap();
        bool allSatisfied = EvaluateRules(placement, out string failReason);

        if (allSatisfied)
        {
            Debug.Log("[WinLose] Win");
            
            onWin.Invoke();
            levelLoader.LoadNextLevel();
        }
        else
        {
            Debug.Log($"[WinLose] Lose — {failReason}");
            
            onLose.Invoke();
            levelLoader.ReloadCurrentLevel();
        }
    }

    private Dictionary<string, TileType> BuildPlacementMap()
    {
        var map = new Dictionary<string, TileType>();
        foreach (var zone in FindObjectsByType<DropZone>())
        {
            string occupantName = zone.OccupantFrogName;
            if (occupantName != null) map[occupantName] = zone.tileType;
        }
        
        return map;
    }

    private bool EvaluateRules(Dictionary<string, TileType> placement, out string failReason)
    {
        foreach (var display in ruleBook.GetDisplays())
        {
            RuleData rule = display.Data;
            if (rule.objectType != RuleObjectType.Tile) continue; // TODO: Handle Other Tiles

            if (!System.Enum.TryParse<TileType>(rule.objectName, true, out TileType target))
            {
                Debug.LogWarning($"[WinLose] Unknown tile name '{rule.objectName}' in rule for {rule.subjectFrogName}");
                continue;
            }

            if (!placement.TryGetValue(rule.subjectFrogName, out TileType actual))
                continue;

            bool satisfied = rule.trueVerb == RuleVerb.Loves 
                ? actual == target
                : actual != target;

            if (!satisfied)
            {
                failReason = $"{rule.subjectFrogName} (true: {rule.trueVerb} {target}) is on {actual}";
                return false;
            }
        }
        failReason = null;
        return true;
    }
}
