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

    private Vector3 _baseScale;
    private Sequence _sequence;

    private void Awake() => _baseScale = transform.localScale;

    private void OnEnable()
    {
        // Start from one extreme so the yoyo loop covers the full range symmetrically.
        transform.localScale = _baseScale * (1f - scaleAmount);
        transform.localRotation = Quaternion.Euler(0f, 0f, -swayAngle);

        _sequence = DOTween.Sequence().SetLink(gameObject);

        _sequence.Join(transform.DOScale(_baseScale * (1f + scaleAmount), scaleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));

        _sequence.Join(transform.DOLocalRotate(new Vector3(0f, 0f, swayAngle), swayDuration)
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
