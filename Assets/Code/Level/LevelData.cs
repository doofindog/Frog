using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Frog/Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject[] customerPrefabs;
    public GameObject dropZoneLayoutPrefab;
    public RuleData[] rules;
}
