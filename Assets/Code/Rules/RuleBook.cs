using System.Collections.Generic;
using UnityEngine;

public class RuleBook : MonoBehaviour
{
    [SerializeField] private RuleDisplay rulePrefab;
    [SerializeField] private Transform rulesContainer;

    private readonly List<RuleDisplay> _displays = new();

    public void LoadRules(RuleData[] rules)
    {
        foreach (var display in _displays)
            Destroy(display.gameObject);
        _displays.Clear();

        if (rules == null) return;

        foreach (var rule in rules)
        {
            var display = Instantiate(rulePrefab, rulesContainer);
            display.Setup(rule);
            _displays.Add(display);
        }
    }

    public IReadOnlyList<RuleDisplay> GetDisplays() => _displays;

    public RuleVerb GetBelievedVerb(RuleDisplay display) =>
        display.IsCorrected ? display.Data.trueVerb : display.Data.statedVerb;
}
