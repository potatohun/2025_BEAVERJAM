using UnityEngine;
using DG.Tweening;

public class WallController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveDistance = -20f;
    [SerializeField] private float moveDelay = 1f;

    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void Start() {
        Invoke("Move", moveDelay);
    }

    private void Move() {
        spriteRenderer.DOFade(1, 1f).OnComplete(() => {
            transform.DOMoveY(transform.position.y + moveDistance, moveSpeed).SetEase(Ease.InSine).SetDelay(moveDelay).OnComplete(() => {
                spriteRenderer.DOFade(0, 1f).OnComplete(() => {
                    transform.position = startPosition;
                    Move();
                });
            });
        });
    }
}
