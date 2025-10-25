using UnityEngine;

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
    
    [Header("Skill Duration Settings")]
    public float cocoSkillDuration = 3f;
    public float totoSkillDuration = 3f;
    public float galileiSkillDuration = 3f;
    public float miuSkillDuration = 3f;
    
    private GameObject currentCharacterFM;
    private Coroutine skillDurationCoroutine;

    [SerializeField] private BoxCollider2D WaterColliderObject;
    
    void Awake()
    {
        // 싱글톤 설정
        if (FM == null)
        {
            FM = this;
            DontDestroyOnLoad(gameObject);
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
            UseSkill(CharacterSkill.Coco);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            UseSkill(CharacterSkill.Toto);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            UseSkill(CharacterSkill.Galilei);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UseSkill(CharacterSkill.Miu);
        }
        
        // 스킬 해금 키 입력 (테스트용)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UnlockSkill(CharacterSkill.Toto);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UnlockSkill(CharacterSkill.Galilei);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UnlockSkill(CharacterSkill.Miu);
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
                UseCocoSkill();
                break;
            default:
                UsePrefabSkill(skill);
                break;
        }
    }
    
    // None 스킬 사용
    void UseNoneSkill()
    {
        currentSkill = CharacterSkill.None;
        SetCocoAnimation(false);
        Debug.Log("지금은 도와주는 친구가 없어요.");
    }
    
    // Coco 스킬 사용 (애니메이션)
    void UseCocoSkill()
    {
        currentSkill = CharacterSkill.Coco;
        SetCocoAnimation(true);
        StartSkillDuration(CharacterSkill.Coco, cocoSkillDuration);
        Debug.Log($"Coco 친구와 함께 해요! (애니메이션) - {cocoSkillDuration}초");
    }
    
    // 프리팹 스킬 사용 (Toto, Galilei, Miu)
    void UsePrefabSkill(CharacterSkill skill)
    {
        SetCocoAnimation(false);
        
        if (!IsSkillUnlocked(skill))
        {
            Debug.Log($"{skill} 친구는 아직 함께하지 못했어요.");
            return;
        }
        
        currentSkill = skill;
        CreateSkillCharacter(skill);
        StartSkillDuration(skill, GetSkillDuration(skill));
        Debug.Log($"{skill} 친구와 함께 해요! - {GetSkillDuration(skill)}초");
    }
    
    // 스킬 캐릭터 생성
    void CreateSkillCharacter(CharacterSkill skill)
    {
        if (Move.Singleton_Move == null) return;
        
        int skillIndex = (int)skill - 2; // None=0, Coco=1이므로 -2 (Toto부터 시작)
        
        if (skillIndex >= 0 && skillIndex < characterPrefabList.Length && characterPrefabList[skillIndex] != null)
        {
            // 플레이어 오브젝트 안에 스킬 캐릭터 생성
            currentCharacterFM = Instantiate(characterPrefabList[skillIndex], Move.Singleton_Move.transform);
            currentCharacterFM.name = $"{skill}_SkillCharacter";
        }
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
    void SetCocoAnimation(bool isActive)
    {
        Move.Singleton_Move?.SetCocoSkill(isActive);
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
        
        // 기존 스킬 캐릭터 제거
        DestroySkillCharacter();
    }
    
    // 스킬 지속 시간 코루틴
    System.Collections.IEnumerator SkillDurationCoroutine(CharacterSkill skill, float duration)
    {
        WaterColliderObject.enabled = true;
        yield return new WaitForSeconds(duration);
        WaterColliderObject.enabled = false;
        // 스킬 시간 종료
        Debug.Log($"{skill} 친구와의 시간이 끝났어요!");
        
        // 스킬 해제
        UseSkill(CharacterSkill.None);
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
    
    // 편의 함수들
    public void UnlockToto() => UnlockSkill(CharacterSkill.Toto);
    public void UnlockGalilei() => UnlockSkill(CharacterSkill.Galilei);
    public void UnlockMiu() => UnlockSkill(CharacterSkill.Miu);
    
    public void UseNone() => UseSkill(CharacterSkill.None);
    public void UseCoco() => UseSkill(CharacterSkill.Coco);
    public void UseToto() => UseSkill(CharacterSkill.Toto);
    public void UseGalilei() => UseSkill(CharacterSkill.Galilei);
    public void UseMiu() => UseSkill(CharacterSkill.Miu);
}
