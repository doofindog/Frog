using UnityEngine;
using DG.Tweening;

/// <summary>
/// Idle "breathing" animation for a pivot transform: slowly scales up and down while
/// swaying its z rotation back and forth. Put this on the pivot object (parent of the
/// label) and it loops on its own.
/// </summary>
public class UIIdlePivot : MonoBehaviour
{
    [Header("Scale")]
    [Tooltip("How far the scale grows/shrinks from its base (e.g. 0.05 = +/-5%).")]
    [SerializeField] private float scaleAmount = 0.05f;
    [Tooltip("Seconds for one half of the scale cycle.")]
    [SerializeField] private float scaleDuration = 1f;

    [Header("Sway")]
    [Tooltip("Max z angle in degrees; sways between -angle and +angle.")]
    [SerializeField] private float swayAngle = 15f;
    [Tooltip("Seconds for one half of the sway cycle.")]
    [SerializeField] private float swayDuration = 1.5f;
    [SerializeField] private bool randomize;

    private Vector3 _baseScale;
    private Sequence _sequence;

    private void Awake() => _baseScale = transform.localScale;

    private void OnEnable()
    {
        float scaleAmt = randomize ? Random.Range(0f, scaleAmount) : scaleAmount;
        float scaleDur = randomize ? Random.Range(Mathf.Max(1f, scaleDuration - 1f), scaleDuration) : scaleDuration;
        float sway = randomize ? Random.Range(0f, swayAngle) : swayAngle;
        float swayDur = randomize ? Random.Range(0f, swayDuration) : swayDuration;

        // Start from one extreme so the yoyo loop covers the full range symmetrically.
        transform.localScale = _baseScale * (1f - scaleAmt);
        transform.localRotation = Quaternion.Euler(0f, 0f, -sway);

        _sequence = DOTween.Sequence().SetLink(gameObject);

        _sequence.Join(transform.DOScale(_baseScale * (1f + scaleAmt), scaleDur)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));

        _sequence.Join(transform.DOLocalRotate(new Vector3(0f, 0f, sway), swayDur)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;

        transform.localScale = _baseScale;
        transform.localRotation = Quaternion.identity;
    }
}
