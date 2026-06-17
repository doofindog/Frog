using UnityEngine;

public enum CustomerMood { Neutral, Happy, Unhappy }

[RequireComponent(typeof(Draggable))]
public class CustomerExpression : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite neutral;
    [SerializeField] private Sprite happy;
    [SerializeField] private Sprite unhappy;

    private Draggable _draggable;

    public string FrogName => _draggable.frogName;

    private void Awake()
    {
        _draggable = GetComponent<Draggable>();
        SetMood(CustomerMood.Neutral);
    }

    public void SetMood(CustomerMood mood)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = mood switch
        {
            CustomerMood.Happy => happy,
            CustomerMood.Unhappy => unhappy,
            _ => neutral,
        };
    }
}
