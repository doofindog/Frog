using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class WordToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text label;

    public string Text => label != null ? label.text : string.Empty;

    private CanvasGroup _canvasGroup;
    private Canvas _rootCanvas;
    private RuleBook _ruleBook;

    private Transform _originParent;
    private int _originSiblingIndex;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(string text, RuleBook ruleBook)
    {
        if (label != null) label.text = text;
        _ruleBook = ruleBook;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_rootCanvas == null)
            _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        _originParent = transform.parent;
        _originSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(_rootCanvas.transform, worldPositionStays: true);
        transform.SetAsLastSibling();

        _canvasGroup.alpha = 0.75f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var rt = (RectTransform)transform;
        rt.anchoredPosition += eventData.delta / _rootCanvas.scaleFactor;

        _ruleBook?.UpdateGapIndicators(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        _ruleBook?.HideAllGapIndicators();

        if (_ruleBook == null) { ReturnToOrigin(); return; }

        Camera cam = GetUICamera();

        RuleDisplay targetDisplay = _ruleBook.GetDisplayAt(eventData.position);

        bool overBank = RectTransformUtility.RectangleContainsScreenPoint(
            (RectTransform)_ruleBook.BankContainer, eventData.position, cam);

        if (targetDisplay != null)
        {
            int index = targetDisplay.GetInsertionIndex(eventData.position);
            transform.SetParent(targetDisplay.SentenceContainer, worldPositionStays: false);
            transform.SetSiblingIndex(index);
        }
        else if (overBank)
        {
            transform.SetParent(_ruleBook.BankContainer, worldPositionStays: false);
        }
        else
        {
            ReturnToOrigin();
        }
    }

    private void ReturnToOrigin()
    {
        transform.SetParent(_originParent, worldPositionStays: false);
        transform.SetSiblingIndex(_originSiblingIndex);
    }

    private Camera GetUICamera()
    {
        if (_rootCanvas == null) return null;
        return _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera;
    }
}
