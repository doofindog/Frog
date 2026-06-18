using System;
using UnityEngine;

public enum RuleVerb { Loves, Hates, Likes }
public enum RuleObjectType { Frog, Tile }
public enum TileType { None, Water, Sun, Shade }

// The ground truth for a single frog condition — used only by WinLoseEvaluator.
[Serializable]
public class WinCondition
{
    public string subjectFrogName;
    public RuleVerb verb;
    public RuleObjectType objectType;
    public string objectName;
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
    public bool blockedEverywhere;
    public string subjectFrog;
    public RuleVerb verb;
    public RuleObjectType objectType;
    public string[] objectNames;

    public bool IsSatisfiedBy(TileType tileType)
    {
        if (blockedEverywhere) return false;
        if (verb == RuleVerb.Likes) return true;

        foreach (var name in objectNames)
        {
            if (!Enum.TryParse<TileType>(name, ignoreCase: true, out TileType target) || target == TileType.None)
                continue;

            if (verb == RuleVerb.Loves && tileType == target) return true;
            if (verb == RuleVerb.Hates && tileType == target) return false;
        }

        return verb == RuleVerb.Hates;
    }

    public bool IsSatisfiedByAdjacency(bool adjacent)
    {
        return verb == RuleVerb.Loves ? adjacent : !adjacent;
    }
}
