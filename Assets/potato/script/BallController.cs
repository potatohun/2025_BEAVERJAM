using UnityEngine;
using DG.Tweening;

public class BallController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int jumpCount = 10;
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float jumpDelay = 1f;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }
    void Start()
    {
        Invoke("Jump", jumpDelay);
    }

    public void Jump() {
        spriteRenderer.DOFade(1, 1f).OnComplete(() => {
        transform.DOJump(transform.position + new Vector3(moveDistance, 0, 0), jumpForce, jumpCount, moveSpeed).SetEase(Ease.Linear).OnComplete(() => {
            spriteRenderer.DOFade(0, 1f).OnComplete(() => {
                    transform.position = startPosition;
                    Jump();
                });
            });
        });
    }
}
