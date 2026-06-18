using System;
using UnityEngine;

public enum RuleVerb { Loves, Hates, Likes }
public enum RuleObjectType { Frog, Tile }
public enum TileType { None, Water, Sun, Shade }

// Word-to-TileType resolution, including synonyms not covered by the enum names themselves.
public static class TileTypeWords
{
    public static bool TryParse(string word, out TileType result)
    {
        if (Enum.TryParse(word, ignoreCase: true, out result) && result != TileType.None)
            return true;

        if (string.Equals(word, "sunlight", StringComparison.OrdinalIgnoreCase))
        {
            result = TileType.Sun;
            return true;
        }

        result = TileType.None;
        return false;
    }
}

// The default words for one sentence row in the rulebook.
[Serializable]
public class SentenceData
{
    public string[] defaultSentence;

    // Index-aligned with defaultSentence. A word is draggable only if its entry here is true.
    public bool[] draggableMask;
}

// Runtime constraint derived from a player-built sentence.
public class RuleConstraint
{
    public string subjectFrog;
    public RuleVerb verb;
    public RuleObjectType objectType;
    public string[] objectNames;

    public bool IsSatisfiedBy(TileType tileType)
    {
        if (verb == RuleVerb.Likes) return true;

        foreach (var name in objectNames)
        {
            if (!TileTypeWords.TryParse(name, out TileType target))
                continue;

            if (verb == RuleVerb.Loves && tileType == target) return true;
            if (verb == RuleVerb.Hates && tileType == target) return false;
        }

        return verb == RuleVerb.Hates;
    }

    public bool IsSatisfiedBy(Func<string, bool> isAdjacentToFrog)
    {
        if (verb == RuleVerb.Likes) return true;

        foreach (var name in objectNames)
        {
            if (verb == RuleVerb.Loves && isAdjacentToFrog(name)) return true;
            if (verb == RuleVerb.Hates && isAdjacentToFrog(name)) return false;
        }

        return verb == RuleVerb.Hates;
    }
}
