using UnityEngine;
using DG.Tweening;

public enum CustomerMood { Neutral, Happy, Unhappy }

[RequireComponent(typeof(Draggable))]
public class CustomerExpression : MonoBehaviour
{
    [SerializeField] private SpriteRenderer frogGraphic;
    [SerializeField] private SpriteRenderer happyBubbleGraphic;
    [SerializeField] private SpriteRenderer unhappyBubbleGraphic;
    [SerializeField] private Sprite neutral;
    [SerializeField] private Sprite happy;
    [SerializeField] private Sprite unhappy;
    [SerializeField] private float bubbleScaleDuration = 0.3f;

    private Draggable _draggable;
    private Vector3 _happyBubbleScale;
    private Vector3 _unhappyBubbleScale;
    private CustomerMood _currentMood = CustomerMood.Neutral;

    public string FrogName => _draggable.frogName;
    public Draggable Draggable => _draggable;

    private void Awake()
    {
        _draggable = GetComponent<Draggable>();

        _happyBubbleScale = happyBubbleGraphic.transform.localScale;
        _unhappyBubbleScale = unhappyBubbleGraphic.transform.localScale;

        happyBubbleGraphic.enabled = false;
        unhappyBubbleGraphic.enabled = false;

        if (frogGraphic != null)
            frogGraphic.sprite = neutral;
    }

    public void SetMood(CustomerMood mood)
    {
        if (mood == _currentMood) return;
        _currentMood = mood;

        if (frogGraphic != null)
        {
            frogGraphic.sprite = mood switch
            {
                CustomerMood.Happy => happy,
                CustomerMood.Unhappy => unhappy,
                _ => neutral,
            };
        }

        ShowBubble(happyBubbleGraphic, _happyBubbleScale, mood == CustomerMood.Happy);
        ShowBubble(unhappyBubbleGraphic, _unhappyBubbleScale, mood == CustomerMood.Unhappy);
    }

    private void ShowBubble(SpriteRenderer bubble, Vector3 baseScale, bool show)
    {
        if (bubble == null) return;

        bubble.transform.DOKill();

        if (show)
        {
            bubble.enabled = true;
            bubble.transform.localScale = Vector3.zero;
            bubble.transform.DOScale(baseScale, bubbleScaleDuration).SetEase(Ease.OutBack);
        }
        else
        {
            bubble.enabled = false;
            bubble.transform.localScale = baseScale;
        }
    }
}
