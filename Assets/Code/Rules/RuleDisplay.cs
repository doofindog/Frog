using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleDisplay : MonoBehaviour
{
    [SerializeField] private Transform sentenceContainer;

    public Transform SentenceContainer => sentenceContainer;

    public void Setup(SentenceData data, WordToken tokenPrefab, RuleBook ruleBook)
    {
        foreach (Transform child in sentenceContainer) Destroy(child.gameObject);

        if (data.defaultSentence != null)
        {
            for (int i = 0; i < data.defaultSentence.Length; i++)
            {
                bool draggable = data.draggableMask != null && i < data.draggableMask.Length && data.draggableMask[i];

                var token = Instantiate(tokenPrefab, sentenceContainer);
                token.Init(data.defaultSentence[i], ruleBook, draggable);

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

    private List<string> GetSentenceWords()
    {
        var words = new List<string>();
        foreach (Transform child in sentenceContainer)
        {
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
}
