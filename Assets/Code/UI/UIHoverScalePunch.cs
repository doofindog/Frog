using UnityEngine;
using DG.Tweening;

/// <summary>Punches the scale when the pointer hovers over the element.</summary>
public class UIHoverScalePunch : UIHoverFeedback
{
    [Header("Scale Punch")]
    [SerializeField] private float punch = 0.15f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private int vibrato = 8;
    [SerializeField] private float elasticity = 1f;

    protected override void OnHoverEnter()
    {
        target.DOKill();
        target.localScale = OriginalScale;
        target.DOPunchScale(Vector3.one * punch, duration, vibrato, elasticity);
    }
}
