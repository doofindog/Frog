using UnityEngine;
using DG.Tweening;

public class CustomerSpawnScale : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private Ease ease = Ease.OutBack;

    private Vector3 _baseScale;

    private void Awake()
    {
        if (target == null)
            target = transform;

        _baseScale = target.localScale;
        target.localScale = Vector3.zero;
    }

    public void Play(float delay = 0f)
    {
        target.DOKill();
        target.localScale = Vector3.zero;
        target.DOScale(_baseScale, duration).SetEase(ease).SetDelay(delay);
    }
}
