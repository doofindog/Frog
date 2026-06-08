using UnityEngine;

[CreateAssetMenu(fileName = "LevelSequence", menuName = "Frog/Level Sequence")]
public class LevelSequence : ScriptableObject
{
    public LevelData[] levels;
}
