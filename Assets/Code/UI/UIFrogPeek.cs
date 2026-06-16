using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIFrogPeek : MonoBehaviour
{
    [SerializeField] private Transform graphic;
    [SerializeField] private float peekDistance = 100f;
    [SerializeField] private float riseDuration = 0.4f;
    [SerializeField] private float dropDuration = 0.4f;
    [SerializeField] private float holdDuration = 0.8f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private float minDelay = 1f;
    [SerializeField] private float maxDelay = 4f;

    private Vector3 _restPos;
    private Vector3 _peekPos;

    private void Awake()
    {
        _restPos = graphic.localPosition;
        // Move along the graphic's own (rotated) up direction instead of the parent's Y.
        Vector3 localUp = graphic.localRotation * Vector3.up;
        _peekPos = _restPos + localUp * peekDistance;
    }

    private void OnEnable()
    {
        graphic.DOKill();
        graphic.localPosition = _restPos;
        StartCoroutine(PeekLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        graphic.DOKill();
        graphic.localPosition = _restPos;
    }

    private IEnumerator PeekLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            yield return graphic.DOLocalMove(_peekPos, riseDuration)
                .SetEase(ease).WaitForCompletion();

            yield return new WaitForSeconds(holdDuration);

            yield return graphic.DOLocalMove(_restPos, dropDuration)
                .SetEase(Ease.InBack).WaitForCompletion();
        }
    }
}
