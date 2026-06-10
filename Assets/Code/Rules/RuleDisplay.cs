using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuleDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text subjectLabel;
    [SerializeField] private Button verbButton;
    [SerializeField] private TMP_Text verbLabel;
    [SerializeField] private TMP_Text objectLabel;

    private RuleData _data;
    private bool _corrected;

    public bool IsCorrected => _corrected;
    public RuleData Data => _data;

    public void Setup(RuleData data)
    {
        _data = data;
        _corrected = false;

        subjectLabel.text = data.subjectFrogName;
        objectLabel.text = data.objectType == RuleObjectType.Frog
            ? data.objectName
            : data.objectName;

        RefreshVerbDisplay();
        verbButton.onClick.RemoveAllListeners();
        verbButton.onClick.AddListener(OnVerbClicked);
    }

    private void OnVerbClicked()
    {
        _corrected = !_corrected;
        RefreshVerbDisplay();
    }

    private void RefreshVerbDisplay()
    {
        if (_corrected)
            verbLabel.text = $"<s>{VerbToString(_data.statedVerb)}</s> {VerbToString(_data.trueVerb)}";
        else
            verbLabel.text = VerbToString(_data.statedVerb);
    }

    private static string VerbToString(RuleVerb verb) => verb switch
    {
        RuleVerb.Loves => "LOVES",
        RuleVerb.Hates => "HATES",
        RuleVerb.Likes => "LIKES",
        _ => verb.ToString().ToUpper()
    };
}
