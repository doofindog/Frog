using UnityEngine;
using DG.Tweening;

/// <summary>Scales the element up while the pointer hovers, easing back on exit.</summary>
public class UIHoverScale : UIHoverFeedback
{
    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease enterEase = Ease.OutBack;
    [SerializeField] private Ease exitEase = Ease.OutQuad;

    protected override void OnHoverEnter()
    {
        target.DOKill();
        target.DOScale(OriginalScale * hoverScale, duration).SetEase(enterEase);
    }

    protected override void OnHoverExit()
    {
        target.DOKill();
        target.DOScale(OriginalScale, duration).SetEase(exitEase);
    }
}
