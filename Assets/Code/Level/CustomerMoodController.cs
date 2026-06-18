using System;
using System.Linq;
using UnityEngine;

public class CustomerMoodController : MonoBehaviour
{
    [SerializeField] private RuleBook ruleBook;

    private DropZoneGrid grid;
    private CustomerExpression[] expressions;

    private void Start()
    {
        DragManager.OnBoardChanged += Refresh;
        RuleBook.OnRulesChanged += Refresh;
    }

    private void OnDestroy()
    {
        DragManager.OnBoardChanged -= Refresh;
        RuleBook.OnRulesChanged -= Refresh;
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
            expression.SetMood(EvaluateMood(expression));
    }

    // Evaluated from this exact instance's own board coordinate, not just its (possibly
    // shared, e.g. species-wide) name — otherwise every customer with the same name would
    // show identical mood regardless of where each one actually sits.
    private CustomerMood EvaluateMood(CustomerExpression expression)
    {
        string frogName = expression.FrogName;
        Vector2Int? coord = grid.FindCoord(expression.Draggable);

        // Not on the board yet
        if (coord == null)
            return CustomerMood.Neutral;

        bool anyPending = false;
        foreach (var constraint in ruleBook.GetAllConstraints())
        {
            if (!string.Equals(constraint.subjectFrog, frogName, StringComparison.OrdinalIgnoreCase))
                continue;

            if (constraint.objectType == RuleObjectType.Tile)
            {
                var zone = grid.GetZone(coord.Value);
                if (zone == null)
                {
                    anyPending = true;
                    continue;
                }

                if (!constraint.IsSatisfiedBy(zone.tileType))
                    return CustomerMood.Unhappy;
            }
            else
            {
                bool allKnown = constraint.objectNames.All(name => grid.FindFrogs(name).Any());
                if (!allKnown)
                {
                    anyPending = true;
                    continue;
                }

                bool satisfied = constraint.IsSatisfiedBy(name => grid.FindFrogs(name).Any(c => DropZoneGrid.AreAdjacent(coord.Value, c)));
                if (!satisfied)
                    return CustomerMood.Unhappy;
            }
        }

        return anyPending ? CustomerMood.Unhappy : CustomerMood.Happy;
    }
}
