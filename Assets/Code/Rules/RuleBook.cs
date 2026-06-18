using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleBook : MonoBehaviour
{
    public static event Action OnRulesChanged;

    [SerializeField] private RuleDisplay rulePrefab;
    [SerializeField] private Transform rulesContainer;
    [SerializeField] private Transform bankContainer;
    [SerializeField] private WordToken wordTokenPrefab;

    private readonly List<RuleDisplay> _displays = new();
    private WordToken _hoveredToken;

    public void NotifyRulesChanged() => OnRulesChanged?.Invoke();

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

    // Returns the WordToken under screenPos (across all sentence rows and the bank), or null.
    public WordToken GetTokenAt(Vector2 screenPos)
    {
        return FindTokenAt(screenPos);
    }

    public void UpdateHoverHighlight(Vector2 screenPos)
    {
        WordToken token = FindTokenAt(screenPos);
        if (token == _hoveredToken) return;

        _hoveredToken?.SetHighlighted(false);
        _hoveredToken = token;
        _hoveredToken?.SetHighlighted(true);
    }

    public void ClearHoverHighlight()
    {
        _hoveredToken?.SetHighlighted(false);
        _hoveredToken = null;
    }

    private WordToken FindTokenAt(Vector2 screenPos)
    {
        Camera cam = GetUICamera();

        foreach (var display in _displays)
        {
            foreach (Transform child in display.SentenceContainer)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)child, screenPos, cam))
                {
                    var token = child.GetComponent<WordToken>();
                    if (token != null && token.Draggable) return token;
                }
            }
        }

        foreach (Transform child in bankContainer)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)child, screenPos, cam))
            {
                var token = child.GetComponent<WordToken>();
                if (token != null && token.Draggable) return token;
            }
        }

        return null;
    }

    public List<RuleConstraint> GetAllConstraints()
    {
        var result = new List<RuleConstraint>();
        foreach (var display in _displays)
            result.AddRange(display.GetCurrentConstraints());
        return result;
    }

    private Camera GetUICamera()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return null;
        var root = canvas.rootCanvas;
        return root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;
    }
}
