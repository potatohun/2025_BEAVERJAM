using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PondController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float drownTime = 6f;
    
    [Header("Pond Object")]
    [SerializeField] private GameObject pond_object;
    
    private bool isFilled = false;
    //private bool isPlayerInWater = false;
    private Coroutine drownCoroutine;

    private GameObject player;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        

        if (FriendManager.FM.currentSkill == FriendManager.CharacterSkill.None)
        {
            if (FriendManager.FM.isPlayerInWater && drownCoroutine == null)
            {
                Debug.Log("플레이어가 물 속에 있습니다!");
                drownCoroutine = StartCoroutine("DrownPlayer");
            }

        }
        else
        {
            if (drownCoroutine != null)
            {
                StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "EyeSensor") 
        {
            Debug.Log("플레이어가 물에 빠졌습니다!");
            FriendManager.FM.isPlayerInWater = true;
            Move.Singleton_Move.rb.gravityScale = 1.5f;
            Move.Singleton_Move.jumpForce = 10f;
            Move.Singleton_Move.maxJumps = 100;
            Move.Singleton_Move.rb.linearDamping = 1.5f;

            Vector2 vel = Move.Singleton_Move.rb.linearVelocity;
            vel.y *= 0.3f;
            Move.Singleton_Move.rb.linearVelocity = vel;

            // 익사 코루틴 시작
            if (drownCoroutine == null) {
                drownCoroutine = StartCoroutine("DrownPlayer");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {

        if (other.gameObject.tag == "EyeSensor") 
        {
            Debug.Log("플레이어가 물에서 나왔습니다!");
            FriendManager.FM.isPlayerInWater = false;
            Move.Singleton_Move.rb.gravityScale = 10f;
            Move.Singleton_Move.jumpForce = 30f;
            Move.Singleton_Move.maxJumps = 1;
            Move.Singleton_Move.rb.linearDamping = 0f;

            Vector2 vel = Move.Singleton_Move.rb.linearVelocity;

            if (vel.y > 0) vel.y *= 1.5f;
            else vel.y = Mathf.Clamp(vel.y, -10f, 0f);

            Move.Singleton_Move.rb.linearVelocity = vel;                               

            // 익사 코루틴 중지
            if (drownCoroutine != null) {
                StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }
        }
    }
    
    private IEnumerator DrownPlayer() {
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
        if(isFilled)
            return;

        isFilled = true;
        pond_object.transform.DOMove(pond_object.transform.position + new Vector3(0f, 5f, 0f), 5f);
    }

}
