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
    
    [Header("State Management")]
    public bool isDead = false;
    public bool isInDialogue = false;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float horizontalInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // FriendManager에 플레이어 참조 설정
        FriendManager.Instance?.SetPlayerReference(this);
        
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
        if(Input.GetKeyDown(KeyCode.Z))
        {
            StartDialogue();
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            EndDialogue();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            SetDead();
        }
        else if(Input.GetKeyDown(KeyCode.V))
        {
            Respawn();
        }
        // 죽은 상태나 대화 중이면 입력 무시
        if (isDead || isInDialogue) return;
        


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
        // 죽은 상태나 대화 중이면 방향 전환 무시
        if (isDead || isInDialogue) return;
        
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
            // Walk 상태: 이동이 있으면 true, 없으면 false (대화 중이면 false)
            bool isWalking = horizontalInput != 0f && !isInDialogue;
            animator.SetBool("Walk", isWalking);
            
            // Dead 상태 설정
            animator.SetBool("Dead", isDead);
        }
    }
    
    // 모든 애니메이션 중앙 관리
    public void SetAllAnimations(bool walk, bool jump, bool cocoSkill, bool dead)
    {
        if (animator != null)
        {
            animator.SetBool("Walk", walk);
            animator.SetBool("Jump", jump);
            animator.SetBool("CocoSkill", cocoSkill);
            animator.SetBool("Dead", dead);
        }
    }
    
    // Idle 상태로 설정 (모든 애니메이션 false)
    public void SetIdleState()
    {
        SetAllAnimations(false, false, false, false);
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
        // 죽은 상태나 대화 중이면 이동 무시
        if (isDead || isInDialogue) return;
        
        // 좌우 이동
        MoveHorizontal();
    }
    
    void MoveHorizontal()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
    
    void Jump()
    {
        // 죽은 상태나 대화 중이면 점프 무시
        if (isDead || isInDialogue) return;
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        // 점프 애니메이션 트리거
        if (animator != null)
        {
            animator.SetBool("Jump", true);
        }
    }
    
    void CheckGrounded()
    {
        // 죽은 상태나 대화 중이면 지면 체크 무시
        if (isDead || isInDialogue) return;
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        
        // 지면에 착지했을 때 점프 상태 해제
        if (isGrounded && animator != null)
        {
            animator.SetBool("Jump", false);
        }
        
        //Debug.Log("isGrounded: " + isGrounded);
    }
    
    // 대화 상태 제어 함수들
    public void StartDialogue()
    {
        isInDialogue = true;
        
        // 모든 애니메이션 중단 (Idle 상태로)
        SetIdleState();
        
        // FriendManager 싱글톤을 통해 스킬 중단
        FriendManager.Instance?.OnDialogueStart();
        
        Debug.Log("대화 시작 - 모든 애니메이션 중단, Idle 상태로 전환");
    }
    
    public void EndDialogue()
    {
        isInDialogue = false;
        Debug.Log("대화 종료 - 캐릭터 움직임 재개");
    }
    
    // 대화 상태 확인
    public bool IsInDialogue()
    {
        return isInDialogue;
    }
    
    // Dead 상태 설정 (기존 기능 유지)
    public void SetDead()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("플레이어가 죽었습니다!");

        // 모든 애니메이션 중단 (Idle 상태로)
        SetIdleState();

        // FriendManager 싱글톤을 통해 스킬 중단
        FriendManager.Instance?.OnDialogueStart();

        // 물리 효과 정지
        rb.linearVelocity = Vector2.zero;
        
        // 애니메이터에 Dead 상태 전달
        if (animator != null)
        {
            animator.SetBool("Dead", true);
        }
    }
    
    // 플레이어 리스폰
    public void Respawn()
    {
        isDead = false;
        Debug.Log("플레이어가 리스폰되었습니다!");
        
        // 애니메이터 상태 리셋
        if (animator != null)
        {
            animator.SetBool("Dead", false);
            animator.SetBool("Jump", false);
        }
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
