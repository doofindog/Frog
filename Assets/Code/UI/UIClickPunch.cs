using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIClickPunch : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform target;
    [SerializeField] private float punch = 0.2f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private int vibrato = 8;
    [SerializeField] private float elasticity = 1f;

    private Vector3 _originalScale;

    private void Awake()
    {
        if (target == null)
            target = transform;

        _originalScale = target.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        target.DOKill();
        target.localScale = _originalScale;
        target.DOPunchScale(Vector3.one * punch, duration, vibrato, elasticity);
    }
}
