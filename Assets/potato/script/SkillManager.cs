using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public List<GameObject> skillList;
    public List<bool> isEndCoolTime;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void UnLockSkill(int skillIndex) {
        skillList[skillIndex].SetActive(true);
    }

    // 전체 UI 쿨타임 활성화
    public void PlaySkill() {
        for(int i = 0; i < skillList.Count; i++) {
            if(skillList[i].activeSelf) {
                CoolTime(i);
            }
        }
    }

    public void CoolTime(int skillIndex) {
        isEndCoolTime[skillIndex] = false;
        StartCoroutine(CoolTimeCoroutine(skillIndex));
    }

    IEnumerator CoolTimeCoroutine(int skillIndex) {
        Debug.Log(skillList[skillIndex].name + " CoolTimeCoroutine");
        SlicedFilledImage filledImage = skillList[skillIndex].GetComponentInChildren<SlicedFilledImage>();
        float coolTime = 5f;
        float elapsedTime = 0f;
        
        // fillAmount를 1로 설정
        filledImage.fillAmount = 1f;
        
        while(elapsedTime < coolTime) {
            elapsedTime += Time.deltaTime;
            
            // fillAmount를 쿨타임 비율에 따라 1에서 0으로 줄여나감
            filledImage.fillAmount = 1f - (elapsedTime / coolTime);
            
            yield return null;
        }
        
        // 쿨타임 완료
        filledImage.fillAmount = 0f;
        isEndCoolTime[skillIndex] = true;
    }

    public bool IsEndCoolTime(int skillIndex) {
        if(skillList[skillIndex].activeSelf == false)
            return false;

        return isEndCoolTime[skillIndex];
    }
}
