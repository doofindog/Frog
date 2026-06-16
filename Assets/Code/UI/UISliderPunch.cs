using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Gives tactile feedback on a slider (e.g. a volume slider) by punching the scale of
/// a target (the handle by default) every time the value changes.
/// </summary>
[RequireComponent(typeof(Slider))]
public class UISliderPunch : MonoBehaviour
{
    [Tooltip("Transform to punch. Defaults to the slider's handle.")]
    [SerializeField] private Transform target;

    [Header("Scale Punch")]
    [SerializeField] private float punch = 0.2f;
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private int vibrato = 8;
    [SerializeField] private float elasticity = 1f;

    private Slider _slider;
    private Vector3 _originalScale;

    private void Awake()
    {
        _slider = GetComponent<Slider>();

        if (target == null && _slider.handleRect != null)
            target = _slider.handleRect;

        if (target != null)
            _originalScale = target.localScale;
    }

    private void OnEnable()  => _slider.onValueChanged.AddListener(HandleValueChanged);
    private void OnDisable() => _slider.onValueChanged.RemoveListener(HandleValueChanged);

    private void HandleValueChanged(float _)
    {
        if (target == null) return;

        target.DOKill();
        target.localScale = _originalScale;
        target.DOPunchScale(Vector3.one * punch, duration, vibrato, elasticity);
    }
}
