using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class WordToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image bg;
    [SerializeField] private Color staticTextColor = new Color(0.78f, 0.74f, 0.66f);
    [SerializeField] private Color draggableTextColor = new Color(0.18f, 0.14f, 0.1f);
    [SerializeField] private UIHoverScalePunch hoverScalePunch;

    public static event System.Action OnDragStarted;
    public static event System.Action OnDropped;

    public string Text => label != null ? label.text : string.Empty;
    public bool Draggable => _draggable;

    private CanvasGroup _canvasGroup;
    private Canvas _rootCanvas;
    private RuleBook _ruleBook;
    private bool _draggable;

    private Transform _originParent;
    private int _originSiblingIndex;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (hoverScalePunch == null) hoverScalePunch = GetComponent<UIHoverScalePunch>();
    }

    public void Init(string text, RuleBook ruleBook, bool draggable = true)
    {
        if (label != null)
        {
            label.text = text;
            label.color = draggable ? draggableTextColor : staticTextColor;
        }
        _ruleBook = ruleBook;
        _draggable = draggable;

        if (hoverScalePunch != null) hoverScalePunch.enabled = draggable;

        if (bg != null && !draggable)
        {
            var color = bg.color;
            color.a = 0f;
            bg.color = color;
        }
    }

    public void SetHighlighted(bool on)
    {
        _canvasGroup.alpha = on ? 0.5f : 1f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_draggable) return;

        if (_rootCanvas == null)
            _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        _originParent = transform.parent;
        _originSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(_rootCanvas.transform, worldPositionStays: true);
        transform.SetAsLastSibling();

        _canvasGroup.alpha = 0.75f;
        _canvasGroup.blocksRaycasts = false;

        OnDragStarted?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_draggable) return;

        var rt = (RectTransform)transform;
        rt.anchoredPosition += eventData.delta / _rootCanvas.scaleFactor;

        _ruleBook?.UpdateHoverHighlight(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_draggable) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        OnDropped?.Invoke();

        _ruleBook?.ClearHoverHighlight();

        if (_ruleBook == null) { ReturnToOrigin(); return; }

        WordToken targetToken = _ruleBook.GetTokenAt(eventData.position);

        if (targetToken != null && targetToken != this)
        {
            Transform targetParent = targetToken.transform.parent;
            int targetIndex = targetToken.transform.GetSiblingIndex();

            targetToken.transform.SetParent(_originParent, worldPositionStays: false);
            targetToken.transform.SetSiblingIndex(_originSiblingIndex);

            transform.SetParent(targetParent, worldPositionStays: false);
            transform.SetSiblingIndex(targetIndex);
            _ruleBook.NotifyRulesChanged();
            return;
        }

        ReturnToOrigin();
    }

    private void ReturnToOrigin()
    {
        transform.SetParent(_originParent, worldPositionStays: false);
        transform.SetSiblingIndex(_originSiblingIndex);
    }
}
