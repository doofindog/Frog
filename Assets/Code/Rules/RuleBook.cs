using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleBook : MonoBehaviour
{
    [SerializeField] private RuleDisplay rulePrefab;
    [SerializeField] private Transform rulesContainer;
    [SerializeField] private Transform bankContainer;
    [SerializeField] private WordToken wordTokenPrefab;

    private readonly List<RuleDisplay> _displays = new();

    public Transform BankContainer => bankContainer;

    public void LoadRules(SentenceData[] sentences, string[] wordBank)
    {
        foreach (var display in _displays)
            Destroy(display.gameObject);
        _displays.Clear();

        foreach (Transform child in bankContainer)
            Destroy(child.gameObject);

        if (sentences != null)
        {
            foreach (var sentence in sentences)
            {
                var display = Instantiate(rulePrefab, rulesContainer);
                display.Setup(sentence, wordTokenPrefab, this);
                _displays.Add(display);
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(display.GetComponent<RectTransform>());
            }
        }

        
        if (wordBank != null)
        {
            foreach (var word in wordBank)
            {
                var token = Instantiate(wordTokenPrefab, bankContainer);
                token.Init(word, this);
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(token.GetComponent<RectTransform>());
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rulesContainer.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(bankContainer.GetComponent<RectTransform>());
    }

    // Returns the RuleDisplay whose sentence area contains screenPos, or null.
    public RuleDisplay GetDisplayAt(Vector2 screenPos)
    {
        Camera cam = GetUICamera();
        foreach (var display in _displays)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    (RectTransform)display.SentenceContainer, screenPos, cam))
                return display;
        }
        return null;
    }

    // Each display handles its own "am I being hovered?" check internally.
    public void UpdateGapIndicators(Vector2 screenPos)
    {
        foreach (var display in _displays)
            display.UpdateGapIndicator(screenPos);
    }

    public void HideAllGapIndicators()
    {
        foreach (var display in _displays)
            display.HideGapIndicator();
    }

    public RuleConstraint GetConstraintForFrog(string frogName)
    {
        foreach (var display in _displays)
        {
            foreach (var constraint in display.GetCurrentConstraints())
            {
                if (string.Equals(constraint.subjectFrog, frogName, StringComparison.OrdinalIgnoreCase))
                    return constraint;
            }
        }
        return null;
    }

    private Camera GetUICamera()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return null;
        var root = canvas.rootCanvas;
        return root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;
    }
}
