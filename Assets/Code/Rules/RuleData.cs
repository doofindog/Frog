using System;
using UnityEngine;

public enum RuleVerb { Loves, Hates }
public enum RuleObjectType { Frog, Tile }
public enum TileType { None, Water, Sun, Shade }

[Serializable]
public class RuleData
{
    public string subjectFrogName;
    public RuleVerb statedVerb;
    public RuleVerb trueVerb;
    public RuleObjectType objectType;
    public string objectName;
}
