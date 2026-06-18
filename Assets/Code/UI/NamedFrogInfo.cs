using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NamedFrogInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI frogNameLabel;
    [SerializeField] private Image frogInfoLabel;
    [SerializeField] private float scaleDuration = 0.3f;

    private Vector3 _baseScale;

    private void Awake() => _baseScale = frogInfoLabel.transform.localScale;

    public void SetFrogInfo(string frogName, Sprite frogGraphic, float delay = 0f)
    {
        frogNameLabel.text = "This is " + frogName;
        frogInfoLabel.sprite = frogGraphic;

        frogNameLabel.enabled = false;
        frogInfoLabel.transform.localScale = Vector3.zero;

        frogInfoLabel.transform.DOScale(_baseScale, scaleDuration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack)
            .OnComplete(() => frogNameLabel.enabled = true);
    }
}
