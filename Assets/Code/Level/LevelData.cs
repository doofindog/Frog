using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Frog/Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject[] customerPrefabs;
    public DropZoneGrid dropZoneLayoutPrefab;
    public SentenceData[] sentences;
    public string[] wordBank;
}
