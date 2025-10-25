using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float horizontalInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Ground check 오브젝트가 없으면 자동으로 생성
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        // 입력 받기 (화살표 키)
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1f;
        
        // 캐릭터 방향 바꾸기
        FlipCharacter();
        
        // 점프 입력 확인
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        
        // 지면 체크
        CheckGrounded();
        
        // 애니메이터 파라미터 업데이트
        UpdateAnimator();
    }
    
    void FlipCharacter()
    {
        if (horizontalInput > 0f)
        {
            // 오른쪽으로 이동 - 정방향 (scale.x = 1)
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (horizontalInput < 0f)
        {
            // 왼쪽으로 이동 - 뒤집기 (scale.x = -1)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
    
    void UpdateAnimator()
    {
        if (animator != null)
        {
            // Walk 상태: 이동이 있으면 true, 없으면 false
            bool isWalking = horizontalInput != 0f;
            animator.SetBool("Walk", isWalking);
        }
    }
    
    // Coco 스킬 애니메이션 제어
    public void SetCocoSkill(bool isActive)
    {
        if (animator != null)
        {
            animator.SetBool("CocoSkill", isActive);
        }
    }
    
    void FixedUpdate()
    {
        // 좌우 이동
        MoveHorizontal();
    }
    
    void MoveHorizontal()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
    
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        // 점프 애니메이션 트리거
        if (animator != null)
        {
            animator.SetBool("Jump", true);
        }
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        
        // 지면에 착지했을 때 점프 상태 해제
        if (isGrounded && animator != null)
        {
            animator.SetBool("Jump", false);
        }
        
        //Debug.Log("isGrounded: " + isGrounded);
    }
    
    
    // 디버그용 - 지면 체크 범위 시각화
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
