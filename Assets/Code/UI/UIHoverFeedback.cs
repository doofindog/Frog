using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Base class for generic DOTween hover feedback on UI elements (buttons, labels, ...).
/// Requires a raycast-target Graphic on the object (e.g. a Button's Image or a TMP label
/// with "Raycast Target" enabled) so pointer events fire. Concrete effects override
/// <see cref="OnHoverEnter"/> / <see cref="OnHoverExit"/>.
/// </summary>
public abstract class UIHoverFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Transform to animate. Defaults to this object's transform.")]
    [SerializeField] protected Transform target;

    protected Vector3 OriginalScale { get; private set; }
    protected Quaternion OriginalRotation { get; private set; }

    protected virtual void Awake()
    {
        if (target == null)
            target = transform;

        OriginalScale = target.localScale;
        OriginalRotation = target.localRotation;
    }

    public void OnPointerEnter(PointerEventData eventData) => OnHoverEnter();
    public void OnPointerExit(PointerEventData eventData) => OnHoverExit();

    protected abstract void OnHoverEnter();
    protected virtual void OnHoverExit() { }
}
