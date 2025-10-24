using UnityEngine;

public class FriendManager : MonoBehaviour
{
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
    
    private GameObject currentCharacterInstance;
    private Transform playerTransform;
    
    void Start()
    {
        // 게임 시작 시 스킬 없음 상태
        currentSkill = CharacterSkill.None;
        Debug.Log("지금은 도와주는 친구가 없어요.");
        
        // 플레이어 Transform 찾기
        playerTransform = FindFirstObjectByType<Move>()?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("플레이어 오브젝트를 찾을 수 없습니다!");
        }
    }
    
    void Update()
    {
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
        // 기존 스킬 캐릭터만 제거
        DestroySkillCharacter();
        
        switch (skill)
        {
            case CharacterSkill.None:
                currentSkill = CharacterSkill.None;
                Debug.Log("지금은 도와주는 친구가 없어요.");
                break;
                
            case CharacterSkill.Coco:
                currentSkill = skill;
                Debug.Log($"{skill} 친구와 함께 해요! (애니메이션)");
                break;
                
            case CharacterSkill.Toto:
            case CharacterSkill.Galilei:
            case CharacterSkill.Miu:
                int skillIndex = (int)skill - 2; // None=0, Coco=1이므로 -2 (Toto부터 시작)
                
                if (skillIndex >= 0 && skillIndex < skillUnlocked.Length && skillUnlocked[skillIndex + 1])
                {
                    currentSkill = skill;
                    CreateSkillCharacter(skill);
                    Debug.Log($"{skill} 친구와 함께 해요!");
                }
                else
                {
                    Debug.Log($"{skill} 친구는 아직 함께하지 못했어요.");
                }
                break;
        }
    }
    
    // 스킬 캐릭터 생성
    void CreateSkillCharacter(CharacterSkill skill)
    {
        if (playerTransform == null) return;
        
        int skillIndex = (int)skill - 2; // None=0, Coco=1이므로 -2 (Toto부터 시작)
        
        if (skillIndex >= 0 && skillIndex < characterPrefabList.Length && characterPrefabList[skillIndex] != null)
        {
            // 플레이어 오브젝트 안에 스킬 캐릭터 생성
            currentCharacterInstance = Instantiate(characterPrefabList[skillIndex], playerTransform);
            currentCharacterInstance.name = $"{skill}_SkillCharacter";
        }
    }
    
    // 스킬 캐릭터만 제거
    void DestroySkillCharacter()
    {
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
            currentCharacterInstance = null;
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
