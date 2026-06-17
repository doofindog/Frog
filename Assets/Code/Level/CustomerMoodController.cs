using System;
using UnityEngine;

public class CustomerMoodController : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    
    private DropZoneGrid grid;
    private CustomerExpression[] expressions;

    private void Start()
    {
        DragManager.OnBoardChanged += Refresh;
        // Refresh when rules change
    }

    private void OnDestroy()
    {
        DragManager.OnBoardChanged -= Refresh;
        // Unsubscribe when rules change
    }

    public void Initialize(DropZoneGrid dropZoneGrid)
    {
        grid = dropZoneGrid;
        expressions = FindObjectsByType<CustomerExpression>();      

        Refresh();
    }

    private void Refresh()
    {
        foreach (var expression in expressions)
            expression.SetMood(EvaluateMood(expression.FrogName));
    }

    private CustomerMood EvaluateMood(string frogName)
    {
        // Not on the board yet
        if (grid.FindFrog(frogName) == null)
            return CustomerMood.Neutral;

        var level = levelLoader.CurrentLevel;
        bool anyPending = false;
        foreach (var condition in level.winConditions)
        {
            if (!string.Equals(condition.subjectFrogName, frogName, StringComparison.OrdinalIgnoreCase))
                continue;
            if (condition.verb == RuleVerb.Likes)
                continue;

            if (condition.objectType == RuleObjectType.Frog)
            {
                var posA = grid.FindFrog(condition.subjectFrogName);
                var posB = grid.FindFrog(condition.objectName);
                if (posA == null || posB == null) 
                { 
                    anyPending = true; 
                    continue; 
                }

                bool adjacent = (posA.Value - posB.Value).sqrMagnitude == 1;
                bool satisfied = condition.verb == RuleVerb.Loves ? adjacent : !adjacent;
                if (!satisfied) 
                    return CustomerMood.Unhappy;
            }
            else
            {
                if (!Enum.TryParse<TileType>(condition.objectName, true, out TileType target))
                    continue;

                var coord = grid.FindFrog(condition.subjectFrogName);
                var zone = coord != null ? grid.GetZone(coord.Value) : null;
                if (zone == null) 
                { 
                    anyPending = true; 
                    continue; 
                }

                bool satisfied = condition.verb == RuleVerb.Loves
                    ? zone.tileType == target
                    : zone.tileType != target;
                if (!satisfied) 
                    return CustomerMood.Unhappy;
            }
        }

        return anyPending ? CustomerMood.Unhappy : CustomerMood.Happy;
    }
}
