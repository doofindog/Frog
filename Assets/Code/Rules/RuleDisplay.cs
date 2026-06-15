using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleDisplay : MonoBehaviour
{
    [SerializeField] private Transform sentenceContainer;

    private RectTransform _gapIndicator;

    public Transform SentenceContainer => sentenceContainer;

    public void Setup(SentenceData data, WordToken tokenPrefab, RuleBook ruleBook)
    {
        foreach (Transform child in sentenceContainer) Destroy(child.gameObject);

        CreateGapIndicator();

        if (data.defaultSentence != null)
        {
            foreach (var word in data.defaultSentence)
            {
                var token = Instantiate(tokenPrefab, sentenceContainer);
                token.Init(word, ruleBook);
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(token.GetComponent<RectTransform>());
            }
        }
    }

    // Returns one RuleConstraint per subject found in the sentence.
    // Returns empty list if sentence can't be parsed.
    public List<RuleConstraint> GetCurrentConstraints()
    {
        var result = new List<RuleConstraint>();
        var words = GetSentenceWords();
        if (words.Count < 3) return result;

        int verbIndex = -1;
        RuleVerb verb = RuleVerb.Likes;
        for (int i = 0; i < words.Count; i++)
        {
            if (Enum.TryParse<RuleVerb>(words[i], ignoreCase: true, out var v))
            {
                verbIndex = i;
                verb = v;
                break;
            }
        }
        if (verbIndex < 0) return result;

        var subjects = ExtractTerms(words, 0, verbIndex);
        var objects = ExtractTerms(words, verbIndex + 1, words.Count);
        if (subjects.Count == 0 || objects.Count == 0) return result;

        bool isTile = false;
        foreach (var obj in objects)
        {
            if (Enum.TryParse<TileType>(obj, ignoreCase: true, out TileType t) && t != TileType.None)
            {
                isTile = true;
                break;
            }
        }

        foreach (var subject in subjects)
        {
            result.Add(new RuleConstraint
            {
                subjectFrog = subject,
                verb = verb,
                objectType = isTile ? RuleObjectType.Tile : RuleObjectType.Frog,
                objectNames = objects.ToArray()
            });
        }

        return result;
    }

    public void UpdateGapIndicator(Vector2 screenPos)
    {
        if (_gapIndicator == null) return;

        Camera cam = GetUICamera();
        bool over = RectTransformUtility.RectangleContainsScreenPoint(
            (RectTransform)sentenceContainer, screenPos, cam);

        if (!over) { _gapIndicator.gameObject.SetActive(false); return; }

        int index = GetInsertionIndex(screenPos);
        _gapIndicator.gameObject.SetActive(true);
        _gapIndicator.SetSiblingIndex(index);
    }

    public void HideGapIndicator()
    {
        if (_gapIndicator != null)
            _gapIndicator.gameObject.SetActive(false);
    }

    public int GetInsertionIndex(Vector2 screenPos)
    {
        Camera cam = GetUICamera();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)sentenceContainer, screenPos, cam, out Vector2 localPos);

        int index = 0;
        int tokenIndex = 0;
        foreach (Transform child in sentenceContainer)
        {
            if (_gapIndicator != null && child == _gapIndicator.transform) continue;

            float midX = child.localPosition.x;
            if (localPos.x > midX)
                index = tokenIndex + 1;

            tokenIndex++;
        }
        return index;
    }

    private List<string> GetSentenceWords()
    {
        var words = new List<string>();
        foreach (Transform child in sentenceContainer)
        {
            if (_gapIndicator != null && child == _gapIndicator.transform) continue;
            var token = child.GetComponent<WordToken>();
            if (token != null && !string.IsNullOrWhiteSpace(token.Text))
                words.Add(token.Text.Trim());
        }
        return words;
    }

    private static List<string> ExtractTerms(List<string> words, int start, int end)
    {
        var terms = new List<string>();
        for (int i = start; i < end; i++)
        {
            if (!words[i].Equals("and", StringComparison.OrdinalIgnoreCase))
                terms.Add(words[i]);
        }
        return terms;
    }

    private void CreateGapIndicator()
    {
        if (_gapIndicator != null) return;

        var go = new GameObject("GapIndicator", typeof(RectTransform), typeof(Image));
        _gapIndicator = go.GetComponent<RectTransform>();
        _gapIndicator.SetParent(sentenceContainer, false);
        _gapIndicator.sizeDelta = new Vector2(4f, 40f);
        go.GetComponent<Image>().color = new Color(0.3f, 0.7f, 1f, 1f);
        go.SetActive(false);
    }

    private Camera GetUICamera()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return null;
        var root = canvas.rootCanvas;
        return root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;
    }
}
