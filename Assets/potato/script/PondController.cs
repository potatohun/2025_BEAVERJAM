using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class PondController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float drownTime = 6f;
    [SerializeField] private bool fillable = false;
    [SerializeField] private float fillTime = 5f;
    [SerializeField] private float fillAmount = 5f;
    
    [Header("Pond Object")]
    [SerializeField] private GameObject pond_object;
    
    private bool isFilled = false;
    //private bool isPlayerInWater = false;
    private Coroutine drownCoroutine;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        
        if(fillable)
        {
            isFilled = false;
        }
        else
        {
            isFilled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 물이 채워지지 않았으면 리턴
        if (isFilled == false)
            return;

        // EyeSensor가 아니면 리턴
        if (other.gameObject.tag != "EyeSensor")
            return;

        // 스킬이 활성화되어 있으면 익사하지 않음
        if (FriendManager.FM.currentSkill == FriendManager.CharacterSkill.Miu)
        {
            // 코루틴이 실행 중이면 중지
            if (drownCoroutine != null)
            {
                StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }
            return;
        }

        // 스킬이 None이고 플레이어가 물에 있으면 코루틴 시작
        if (drownCoroutine == null)
        {
            Debug.Log("플레이어가 물 속에 있습니다!");
            drownCoroutine = StartCoroutine("DrownPlayer");

            // 플레이어 물에 빠지기
            Move.Singleton_Move.StartDrownPlayer();
        }
        
        Debug.Log("STAY : 플레이어가 물 속에 있습니다!");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // 물이 채워지지 않았으면 리턴
        if (isFilled == false)
            return;

        if (other.gameObject.tag == "EyeSensor")
        {
            // 플레이어 물에 빠지기
            Move.Singleton_Move.StartDrownPlayer();

            // 스킬이 활성화되어 있으면 익사 코루틴 시작하지 않음
            if (FriendManager.FM.currentSkill == FriendManager.CharacterSkill.Miu)
            {
                return;
            }

            // 익사 코루틴 시작 (이미 실행 중이면 재시작하지 않음)
            if (drownCoroutine == null)
            {
                drownCoroutine = StartCoroutine("DrownPlayer");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        // 물이 채워지지 않았으면 리턴
        if (isFilled == false)
            return;

        if (other.gameObject.tag == "EyeSensor")
        {
            // 플레이어 물에서 나오기
            Move.Singleton_Move.StopDrownPlayer();

            // 익사 코루틴 중지
            if (drownCoroutine != null)
            {
                StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }
        }
    }
    
    private IEnumerator DrownPlayer() {
        // 물이 채워지지 않았으면 리턴
        if (isFilled == false)
            yield break;

        Debug.Log($"플레이어가 {drownTime}초 후에 익사합니다...");

        yield return new WaitForSeconds(drownTime);

        // 플레이어가 여전히 물에 있다면 죽임
        if (FriendManager.FM.isPlayerInWater) {
            Debug.Log("플레이어가 익사했습니다!");
            Move.Singleton_Move.SetDead();
        }
        
        drownCoroutine = null;
    }

    public void FillPond() {
        if(isFilled == true || fillable == false)
            return;

        isFilled = true;

        pond_object.transform.DOMove(pond_object.transform.position + new Vector3(0f, fillAmount, 0f), fillTime);
    }

}
