using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PondController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float drownTime = 10f;
    
    [Header("Pond Object")]
    [SerializeField] private GameObject pond_object;
    
    private bool isFilled = false;
    private bool isPlayerInWater = false;
    private Coroutine drownCoroutine;

    private GameObject player;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
    }

    

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            Debug.Log("플레이어가 물에 빠졌습니다!");
            isPlayerInWater = true;
            
            // 익사 코루틴 시작
            if(drownCoroutine == null) {
                drownCoroutine = StartCoroutine(DrownPlayer());
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            Debug.Log("플레이어가 물에서 나왔습니다!");
            isPlayerInWater = false;
            
            // 익사 코루틴 중지
            if(drownCoroutine != null) {
                StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }
        }
    }
    
    private IEnumerator DrownPlayer() {
        Debug.Log($"플레이어가 {drownTime}초 후에 익사합니다...");
        
        yield return new WaitForSeconds(drownTime);
        
        // 플레이어가 여전히 물에 있다면 죽임
        if(isPlayerInWater) {
            Debug.Log("플레이어가 익사했습니다!");
            Move.Singleton_Move.SetDead();
        }
        
        drownCoroutine = null;
    }

    public void FillPond() {
        if(isFilled)
            return;

        isFilled = true;
        pond_object.transform.DOMove(pond_object.transform.position + new Vector3(0f, 5f, 0f), 5f);
    }
}
