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
    private CircleCollider2D circleCollider;
    private Vector3 startPosition;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        startPosition = transform.position;
    }
    void Start()
    {
        Invoke("Jump", jumpDelay);
    }

    public void Jump() {
        // 콜라이더 활성화
        circleCollider.enabled = true;

        // 공 투명도 1로 변경
        spriteRenderer.DOFade(1, 1f).OnComplete(() => {
        // 공 이동
        transform.DOJump(transform.position + new Vector3(moveDistance, 0, 0), jumpForce, jumpCount, moveSpeed).SetEase(Ease.Linear).OnComplete(() => {
            // 콜라이더 비활성화
            circleCollider.enabled = false;

            // 공 투명도 0으로 변경
            spriteRenderer.DOFade(0, 1f).OnComplete(() => {
                    // 공 위치 초기화
                    transform.position = startPosition;
                    // 재시작
                    Jump();
                });
            });
        });
    }
}
