using UnityEngine;
using DG.Tweening;

/// <summary>Punches the z-axis rotation when the pointer hovers over the element.</summary>
public class UIHoverRotationPunch : UIHoverFeedback
{
    [Header("Rotation Punch")]
    [SerializeField] private float punchAngle = 15f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private int vibrato = 5;
    [SerializeField] private float elasticity = 1f;

    protected override void OnHoverEnter()
    {
        target.DOKill();
        target.localRotation = OriginalRotation;
        target.DOPunchRotation(new Vector3(0f, 0f, punchAngle), duration, vibrato, elasticity);
    }
}
