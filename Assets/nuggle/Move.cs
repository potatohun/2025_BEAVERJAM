using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class Move : MonoBehaviour
{
    public static Move Singleton_Move { get; private set; }
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 30f;
    
    [Header("Ground Check")]
    public LayerMask groundLayerMask;
    public float groundCheckDistance = 1f;
    public Transform groundCheckPoint; // 레이캐스트 시작점
    
    //public LayerMask waterLayerMask;
    //public bool Playerinwater = false;


    [Header("State Management")]
    public bool isDead = false;
    public bool isInDialogue = false;
    
    [Header("Death Layers")]
    public LayerMask deathLayerMask; // Fire(6번), Obstacle(7번) 레이어들
    
    public Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float horizontalInput;
    
    // 2단 점프 관련 변수
    private int jumpCount = 0;
    private int maxJumps = 1; // 최대 2단 점프

    public GameObject WaterCol_Object;

    float waterTimer = 0.0f;
    
    void Start()
    {
        if (Singleton_Move == null)
        {
            Singleton_Move = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // DeathLayerMask 자동 설정 (Fire=6, Obstacle=7)
        if (deathLayerMask.value == 0)
        {
            deathLayerMask = (1 << 6) | (1 << 7); // Fire(6번) + Obstacle(7번)
            Debug.Log($"DeathLayerMask 자동 설정됨: {deathLayerMask.value} (Fire=6, Obstacle=7)");
        }
        
        // Ground check 포인트가 없으면 자동으로 생성
        if (groundCheckPoint == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheckPoint");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheckPoint = groundCheckObj.transform;
        }
    }

    void Update()
    {
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
        
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // 애니메이터 파라미터 업데이트
        UpdateAnimator();
        isGrounded = CheckGrounded();
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
            // 죽은 상태면 Walk와 Jump 애니메이션 강제로 false
            if (isDead)
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Dead", true);
                return; // 죽은 상태면 다른 애니메이션 업데이트 중단
            }
            
            // Walk 상태: 이동이 있으면 true, 없으면 false (대화 중이면 false)
            bool isWalking = horizontalInput != 0f && !isInDialogue;
            animator.SetBool("Walk", isWalking);
            
            // Dead 상태 설정
            animator.SetBool("Dead", isDead);
            
            // 스킬 애니메이션 중이면 다른 상태는 건드리지 않음
            bool isCocoSkillActive = animator.GetBool("CocoSkill");
            if (isCocoSkillActive)
            {
                return; // 스킬 애니메이션 중이면 Walk와 Dead만 업데이트하고 종료
            }
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
    public void SetCocoSkill(bool isActive, int currentFriendID)
    {
        if (animator != null)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("CocoSkill", isActive);
            animator.SetInteger("FriendID", currentFriendID);
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
        if (isDead || isInDialogue || FriendManager.FM.currentSkill == FriendManager.CharacterSkill.Galilei) return;
        
        // 점프 가능 조건: 땅에 있거나 점프 횟수가 최대보다 적을 때
        if (isGrounded || jumpCount < maxJumps)
        {
            // 공중에서 점프할 때만 점프 횟수 증가
            if (!isGrounded)
            {
                jumpCount++;
                Debug.Log($"공중 점프! 점프 횟수: {jumpCount}/{maxJumps}");

                SoundManager.instance.PlaySound("jump");
            }
            else
            {
                SoundManager.instance.PlaySound("jump");
                Debug.Log("지면에서 점프!");
            }
            
            // 점프 실행
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            // 점프 애니메이션 트리거
            if (animator != null)
            {
                animator.SetBool("Jump", true);
            }
        }
        else
        {
            Debug.Log($"점프 불가! 점프 횟수 초과: {jumpCount}/{maxJumps}");
        }
    }
    
    
    // Ground 상태 확인 (외부에서 호출용)
    public bool IsGrounded()
    {
        return CheckGrounded();
    }
    
    // 레이캐스트 기반 지면 체크
    private bool CheckGrounded()
    {
        if (groundCheckPoint == null) return false;
        
        // 발 아래로 레이캐스트 발사
        Vector2 rayOrigin = groundCheckPoint.position;
        Vector2 rayDirection = Vector2.down;
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, groundCheckDistance, groundLayerMask);

        bool grounded = hit.collider != null;
        if(FriendManager.FM.currentSkill != FriendManager.CharacterSkill.Miu)
            animator.SetBool("Jump", !grounded);
        // 디버그용 레이캐스트 시각화
        Debug.DrawRay(rayOrigin, rayDirection * groundCheckDistance, grounded ? Color.green : Color.red);

        if (grounded) jumpCount = 0;
        //else
        //{
        //    RaycastHit2D hit_water = Physics2D.Raycast(rayOrigin, rayDirection, groundCheckDistance, waterLayerMask);
        //    Playerinwater = hit_water.collider != null;
        //}


        return grounded;
    }

    // 죽음 트리거 처리 (상대편이 Fire(6번)나 Obstacle(7번) 레이어면 죽음)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!WaterCol_Object.GetComponent<CircleCollider2D>().enabled)
        {
            Debug.Log($"[충돌 감지] 플레이어와 충돌한 오브젝트: {other.gameObject.name}, 레이어: {other.gameObject.layer}");

            // 이미 죽었거나 대화 중이면 무시
            if (isDead || isInDialogue)
            {
                Debug.Log("[무시] 이미 죽었거나 대화 중");
                return;
            }

            // 상대편이 Fire나 Obstacle 레이어인지 확인
            if (IsDeathLayer(other.gameObject))
            {
                Debug.Log($"[죽음] 플레이어가 {other.gameObject.name}({GetLayerName(other.gameObject.layer)})과 충돌하여 죽었습니다!");
                SetDead();
            }
            else
            {
                Debug.Log($"[안전] 충돌한 오브젝트는 안전한 레이어입니다: {GetLayerName(other.gameObject.layer)}");
            }
        }

    }

    // 죽음 레이어인지 확인 (상대편이 Fire(6번)나 Obstacle(7번) 레이어인지 체크)
    private bool IsDeathLayer(GameObject obj)
    {
        int objLayer = obj.layer;
        int deathLayerMaskValue = deathLayerMask.value;
        
        bool isDeathLayer = (deathLayerMaskValue & (1 << objLayer)) != 0;
        
        Debug.Log($"[레이어 체크] 오브젝트: {obj.name}, 레이어: {objLayer}({GetLayerName(objLayer)}), DeathLayerMask: {deathLayerMaskValue}, 결과: {isDeathLayer} (Fire=6, Obstacle=7)");
        
        return isDeathLayer;
    }
    
    // 레이어 번호를 레이어 이름으로 변환
    private string GetLayerName(int layerIndex)
    {
        return LayerMask.LayerToName(layerIndex);
    }
    
    // 대화 상태 제어 함수들
    public void StartDialogue()
    {
        isInDialogue = true;
        
        // 모든 애니메이션 중단 (Idle 상태로)
        SetIdleState();
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // FriendManager 싱글톤을 통해 스킬 중단
        FriendManager.FM?.OnDialogueStart();
        
        Debug.Log("대화 시작 - 모든 애니메이션 중단, Idle 상태로 전환");
    }
    
    public void EndDialogue()
    {
        isInDialogue = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Debug.Log("대화 종료 - 캐릭터 움직임 재개");
    }
    
    // 대화 상태 확인
    public bool IsInDialogue()
    {
        return isInDialogue;
    }
    
    // Dead 상태 설정 (움직임과 스킬 차단)
    public void SetDead()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("플레이어가 죽었습니다!");
        SoundManager.instance.PlaySound("gameover");

        // 물리 효과 정지
        rb.linearVelocity = Vector2.zero;
        
        // 애니메이터에 Dead 상태 전달 (애니메이션은 유지)
        if (animator != null)
        {
            animator.SetBool("Dead", true);
        }
        
        // FriendManager 싱글톤을 통해 스킬 중단
        FriendManager.FM?.OnDialogueStart();
        GameManager.instance.GameOver();

        this.GetComponent<CircleCollider2D>().enabled = false;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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
            animator.SetBool("CocoSkill", false);
            animator.SetBool("Walk", false);
        }

        this.GetComponent<CircleCollider2D>().enabled = true;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
    
    // 디버그용 - 지면 체크 트리거 시각화

}
