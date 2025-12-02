using UnityEngine;
using System.Collections;

public class FriendManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static FriendManager FM { get; private set; }
    
    public enum CharacterSkill
    {
        None,
        Coco,
        Toto,
        Galilei,
        Miu
    }
    
    [Header("Skill Unlock Status")]
    public bool[] skillUnlocked = { true, false, false, false }; // Coco, Toto, Galilei, Miu
    
    [Header("Current Active Skill")]
    public CharacterSkill currentSkill = CharacterSkill.None;

    [Header("Character Prefab List")]
    public GameObject[] characterPrefabList; // Toto, Galilei, Miu 순서
    public GameObject[] ParticlePrefab;
    
    [Header("Skill Duration Settings")]
    public float cocoSkillDuration = 3f;
    public float totoSkillDuration = 2f;
    public float galileiSkillDuration = 1f;
    public float miuSkillDuration = 5f;
    
    private GameObject currentCharacterFM;
    private Coroutine skillDurationCoroutine;

    private GameObject Particle;

    [Header("Galilei Skill Settings")]
    public float throwForceX = 7f;   // 수평 던지는 힘
    public float throwForceY = 14f;  // 수직 던지는 힘

    [SerializeField] private CircleCollider2D WaterColliderObject;
    public bool isPlayerInWater = false;

    void Awake()
    {
        // 싱글톤 설정
        if (FM == null)
        {
            FM = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // 게임 시작 시 스킬 없음 상태
        currentSkill = CharacterSkill.None;
        Debug.Log("지금은 도와주는 친구가 없어요.");
    }
    
    void Update()
    {
        // Move 싱글톤 직접 참조로 최적화
        if (Move.Singleton_Move == null || Move.Singleton_Move.isDead || Move.Singleton_Move.isInDialogue) return;
        
        // 스킬 사용 키 입력
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skillUnlocked[0] && SkillManager.instance.IsEndCoolTime(0) && !isPlayerInWater)
                UseSkill(CharacterSkill.Coco);
            else 
                return;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (skillUnlocked[1] && Move.Singleton_Move.IsGrounded() && SkillManager.instance.IsEndCoolTime(1) && !isPlayerInWater)
                UseSkill(CharacterSkill.Toto);
            else 
                return;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (skillUnlocked[2] && Move.Singleton_Move.IsGrounded() && SkillManager.instance.IsEndCoolTime(2) && !isPlayerInWater)
                UseSkill(CharacterSkill.Galilei);
            else 
                return;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (skillUnlocked[3] && isPlayerInWater && SkillManager.instance.IsEndCoolTime(3))
                UseSkill(CharacterSkill.Miu);
            else 
                return;
        }

        if(currentSkill == CharacterSkill.Miu && isPlayerInWater)
        {
            Move.Singleton_Move.moveSpeed = 20f;  //30
        }
        else if(currentSkill == CharacterSkill.Miu && !isPlayerInWater)
        {
            StopCurrentSkill();
        }

        
    }
    
    // 스킬 해금
    public void UnlockSkill(CharacterSkill skill)
    {
        int skillIndex = (int)skill - 1;
        if (skillIndex >= 0 && skillIndex < skillUnlocked.Length)
        {
            skillUnlocked[skillIndex] = true;
            Debug.Log($"{skill} 친구와 함께 달릴 수 있어요!");
            SkillManager.instance.UnLockSkill(skillIndex);
            NotiManager.instance.ShowNoti(0);
        }
    }
    
    // 스킬 사용
    public void UseSkill(CharacterSkill skill)
    {
        StopCurrentSkill();

        switch (skill)
        {
            case CharacterSkill.None:
                UseNoneSkill();
                break;
            case CharacterSkill.Coco:
            case CharacterSkill.Toto:
            case CharacterSkill.Galilei:
            case CharacterSkill.Miu:
                UseFriendsSkill(skill);
                break;
            default: break;
        }
    }

    public void Finish_GalileiSkill()
    {
        StopCurrentSkill();
        SetCocoAnimation(false, 0);
    }

    // None 스킬 사용
    void UseNoneSkill()
    {
        currentSkill = CharacterSkill.None;
        SetCocoAnimation(false, 0);
        Debug.Log("지금은 도와주는 친구가 없어요.");
    }
   
    
    // 프리팹 스킬 사용 (Toto, Galilei, Miu)
    void UseFriendsSkill(CharacterSkill skill)
    {
        currentSkill = skill;

        SetCocoAnimation(true, (int)skill);
        
        if (!IsSkillUnlocked(skill))
        {
            Debug.Log($"{skill} 친구는 아직 함께하지 못했어요.");
            return;
        }
        if(skill != CharacterSkill.Coco)
            CreateSkillCharacter(skill); // 스킬 사용시 파티클 생성
        StartSkillDuration(skill, GetSkillDuration(skill));  // 스킬 지속시간 지나면 종료 파티클 생성
         Debug.Log($"{skill} 친구와 함께 해요! - {GetSkillDuration(skill)}초");
        SkillManager.instance.PlaySkill();
        switch(skill){
            case CharacterSkill.Coco:
                SoundManager.instance.PlaySound("skill");
                StartSkillDuration(CharacterSkill.Coco, cocoSkillDuration);
                Debug.Log($"Coco 친구와 함께 해요! (애니메이션) - {cocoSkillDuration}초");
                break;
            case CharacterSkill.Toto:
                if (Move.Singleton_Move != null){
                    SoundManager.instance.PlaySound("skill");
                    Move.Singleton_Move.moveSpeed = 30f;
                    Move.Singleton_Move.rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                }
                break;
            case CharacterSkill.Galilei:
                Move.Singleton_Move.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                StartCoroutine(GalileiSkillSequence());
                break;
            case CharacterSkill.Miu:
                //Move.Singleton_Move.moveSpeed = 30f;
                //Move.Singleton_Move.Playerinwater = false;
                SetCocoAnimation(true, 4);
                SoundManager.instance.PlaySound("skill");
                break;
        }

    }
    
    // 스킬 캐릭터 생성
    void CreateSkillCharacter(CharacterSkill skill)
    {
        if (Move.Singleton_Move == null) return;
        
        int skillIndex = -1;

        switch (skill)
        {
            case CharacterSkill.None:
            case CharacterSkill.Coco:
                skillIndex = -1;
                break;
            case CharacterSkill.Galilei: skillIndex = 0; break;
            case CharacterSkill.Toto: skillIndex = 1; break;
            case CharacterSkill.Miu: skillIndex = 2; break;
        }

        if (skillIndex > 0)
        {
            // 플레이어 오브젝트 안에 스킬 캐릭터 생성
            currentCharacterFM = Instantiate(characterPrefabList[skillIndex-1], Move.Singleton_Move.transform);
            currentCharacterFM.name = $"{skill}_SkillCharacter";

            Particle = Instantiate(ParticlePrefab[0], currentCharacterFM.transform);


            //// Galilei일 경우 x 위치를 -2으로 설정
            //if (skill == CharacterSkill.Galilei)
            //{
            //    Vector3 GalileiPosition = currentCharacterFM.transform.localPosition;
            //    GalileiPosition.y = -2f;
            //    currentCharacterFM.transform.localPosition = GalileiPosition;
            //    Debug.Log($"Galilei 스킬 캐릭터 생성 - y 위치: {GalileiPosition.y}");
            //}
            // Miu일 경우 x 위치를 4.6으로 설정
            if (skill == CharacterSkill.Miu)
            {
                Vector3 miuPosition = currentCharacterFM.transform.localPosition;
                miuPosition.x = 4.6f;
                currentCharacterFM.transform.localPosition = miuPosition;
                Debug.Log($"Miu 스킬 캐릭터 생성 - x 위치: {miuPosition.x}");
            }

        }
        else if(skillIndex == 0) { Particle = Instantiate(ParticlePrefab[0], Move.Singleton_Move.transform); }


    }
    
    // 스킬 캐릭터만 제거
    void DestroySkillCharacter()
    {
        if (currentCharacterFM != null)
        {
            Destroy(currentCharacterFM);
            currentCharacterFM = null;
        }
    }
    
    // Coco 애니메이션 제어
    void SetCocoAnimation(bool isActive, int currentFriendID)
    {
        Move.Singleton_Move?.SetCocoSkill(isActive, currentFriendID);
    }

    // Galilei 스킬 시퀀스 코루틴 - 단순하게 위로 던지기
    IEnumerator GalileiSkillSequence()
    {
        if (Move.Singleton_Move == null) yield break;

        SoundManager.instance.PlaySound("skill");

        Debug.Log("Galilei 스킬 시작 - 1초 후 던지기!");

        
        // 1초 대기
        yield return new WaitForSeconds(GetSkillDuration(CharacterSkill.Galilei));
        
        Debug.Log("Galilei 스킬 - 위로 던지기 실행!");
        Vector3 pos = Move.Singleton_Move.transform.position;
        Move.Singleton_Move.transform.position = pos + new Vector3(0f, 2f, 0f);
        Move.Singleton_Move.isThrown = true;
        Move.Singleton_Move.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Rigidbody2D rb = Move.Singleton_Move.rb;
        rb.linearVelocity = Vector2.zero;

        float direction = transform.localScale.x > 0 ? 1 : -1;
        Vector2 throwVelocity = new Vector2(throwForceX * direction, throwForceY);
        rb.linearVelocity = throwVelocity;

        //// Gravity Scale이 10이므로 매우 강한 힘 필요
        //float throwForce = 30f; // 높은 속도
        //float forceMultiplier = 80f; // 충분히 큰 힘

        //// 즉각적인 속도 적용
        //rb.linearVelocity = new Vector2(rb.linearVelocity.x, throwForce);

        //// 강한 힘 적용 (Gravity Scale 10에 맞춰)
        //rb.AddForce(new Vector2(0, throwForce * forceMultiplier), ForceMode2D.Force);

        //Debug.Log($"Galilei 스킬 - 위로 던졌습니다! (속도: {throwForce}, 힘: {throwForce * forceMultiplier})");
    }
    
    
    // 스킬 지속 시간 시작
    void StartSkillDuration(CharacterSkill skill, float duration)
    {
        if (skillDurationCoroutine != null)
        {
            StopCoroutine(skillDurationCoroutine);
        }
        skillDurationCoroutine = StartCoroutine(SkillDurationCoroutine(skill, duration));
    }
    
    // 현재 스킬 정지
    void StopCurrentSkill()
    {
        if (skillDurationCoroutine != null)
        {
            StopCoroutine(skillDurationCoroutine);
            skillDurationCoroutine = null;
        }

        switch(currentSkill){
            case CharacterSkill.Toto:
                if (Move.Singleton_Move != null){
                    Move.Singleton_Move.moveSpeed = 10f;
                    Move.Singleton_Move.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                break;
            case CharacterSkill.Galilei:
                //// Galilei 스킬 중단 시 특별 처리
                //if (Move.Singleton_Move != null)
                //{
                //    Rigidbody2D rb = Move.Singleton_Move.rb;
                //    rb.bodyType = RigidbodyType2D.Dynamic;
                //    rb.simulated = true;
                //    rb.linearVelocity = Vector2.zero;
                //    rb.angularVelocity = 0f;
                //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                //    Move.Singleton_Move.moveSpeed = 10f;

                //    // 애니메이션 상태 복원
                //    SetCocoAnimation(false, 0);
                //}
                SetCocoAnimation(false, 0);
                break;
            case CharacterSkill.Miu:
                if (Move.Singleton_Move != null)
                {
                    Move.Singleton_Move.moveSpeed = 10f;
                }
                UseNoneSkill();
                break;
        }
        // 기존 스킬 캐릭터 제거
        DestroySkillCharacter();
    }
    
    // 스킬 지속 시간 코루틴
    System.Collections.IEnumerator SkillDurationCoroutine(CharacterSkill skill, float duration)
    {
        float particletime = duration - 0.5f;
        if(skill == CharacterSkill.Coco)
        {
            WaterColliderObject.enabled = true;
        }

        Debug.Log(particletime);

        yield return new WaitForSeconds(particletime);
        if(currentSkill == CharacterSkill.Galilei) Particle = Instantiate(ParticlePrefab[1], Move.Singleton_Move.transform);
        else if(currentSkill != CharacterSkill.Coco)
            Particle = Instantiate(ParticlePrefab[1], currentCharacterFM.transform);
        yield return new WaitForSeconds(0.5f);
        WaterColliderObject.enabled = false;
        // 스킬 시간 종료
        Debug.Log($"{skill} 친구와의 시간이 끝났어요!");
        
        switch(skill){
            case CharacterSkill.Coco: UseSkill(CharacterSkill.None); break;
            case CharacterSkill.Toto:
                if (Move.Singleton_Move != null){
                    Move.Singleton_Move.moveSpeed = 10f;
                    Move.Singleton_Move.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    if (Move.Singleton_Move.rb.IsSleeping()) Move.Singleton_Move.rb.WakeUp();
                }
                UseSkill(CharacterSkill.None);
                break;
            case CharacterSkill.Galilei:
                //// Galilei 스킬 종료 시 특별 처리
                //if (Move.Singleton_Move != null)
                //{
                //    Rigidbody2D rb = Move.Singleton_Move.rb;
                //    rb.bodyType = RigidbodyType2D.Dynamic;
                //    rb.simulated = true;
                //    rb.linearVelocity = Vector2.zero;
                //    rb.angularVelocity = 0f;
                //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                //    Move.Singleton_Move.moveSpeed = 10f;

                //    // 애니메이션 상태 복원
                //    SetCocoAnimation(false, 0);
                //}
                Finish_GalileiSkill();
                break;
            case CharacterSkill.Miu:
                if (Move.Singleton_Move != null){
                    Move.Singleton_Move.moveSpeed = 10f;
                }
                UseSkill(CharacterSkill.None);
                break;
        }

    }
    
    // 스킬별 지속 시간 가져오기
    float GetSkillDuration(CharacterSkill skill)
    {
        switch (skill)
        {
            case CharacterSkill.Coco: return cocoSkillDuration;
            case CharacterSkill.Toto: return totoSkillDuration;
            case CharacterSkill.Galilei: return galileiSkillDuration;
            case CharacterSkill.Miu: return miuSkillDuration;
            default: return 0f;
        }
    }
    
    // 현재 스킬 확인
    public CharacterSkill GetCurrentSkill()
    {
        return currentSkill;
    }
    
    // 특정 스킬이 해금되었는지 확인
    public bool IsSkillUnlocked(CharacterSkill skill)
    {
        if (skill == CharacterSkill.None) return true; // None은 항상 사용 가능
        
        int skillIndex = (int)skill - 1; // None이 0이므로 -1
        return skillIndex >= 0 && skillIndex < skillUnlocked.Length && skillUnlocked[skillIndex];
    }
    
    // 대화 상태에 따른 스킬 중단 처리
    public void OnDialogueStart()
    {
        if (currentSkill != CharacterSkill.None)
        {
            // 코루틴 중단
            StopCurrentSkill();
            
            // 스킬 상태 초기화
            currentSkill = CharacterSkill.None;
            
            Debug.Log("대화 시작으로 인한 스킬 중단 - 코루틴 중단");
        }
    }
    
}
