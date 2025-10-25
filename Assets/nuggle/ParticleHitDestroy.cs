using UnityEngine;
using System.Collections;

public class ParticleHitDestroy : MonoBehaviour
{
    [Header("Fire Collision Settings")]
    public bool destroyOnFireCollision = true;
    public float shrinkDuration = 2f;
    public float destroyThreshold = 0.4f;
    
    [Header("Debug")]
    public bool showDebugLog = true;
    
    private Coroutine shrinkCoroutine;
    private GameObject currentFireObject;
    private bool isShrinking = false;
    private int fireLayer;
    
    void Awake()
    {
        // 레이어 캐싱으로 성능 최적화
        fireLayer = LayerMask.NameToLayer("Fire");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // 캐싱된 레이어로 빠른 비교
        if (other.gameObject.layer == fireLayer)
        {
            if (showDebugLog)
                Debug.Log($"{gameObject.name}이 Fire 레이어와 충돌했습니다!");
            
            if (destroyOnFireCollision && !isShrinking)
            {
                StartShrinking(other.gameObject);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == fireLayer)
        {
            if (showDebugLog)
                Debug.Log($"{gameObject.name}이 Fire 레이어와 충돌이 끝났습니다!");
            
            if (isShrinking && currentFireObject == other.gameObject)
            {
                StopShrinking();
            }
        }
    }
    
    void StartShrinking(GameObject fireObject)
    {
        currentFireObject = fireObject;
        isShrinking = true;
        shrinkCoroutine = StartCoroutine(ShrinkAndDestroy(fireObject));
    }
    
    void StopShrinking()
    {
        if (shrinkCoroutine != null)
        {
            StopCoroutine(shrinkCoroutine);
            shrinkCoroutine = null;
        }
        
        ResetState();
        
        if (showDebugLog)
            Debug.Log("Fire 오브젝트 축소가 중단되었습니다!");
    }
    
    void ResetState()
    {
        isShrinking = false;
        currentFireObject = null;
        shrinkCoroutine = null;
    }
    
    IEnumerator ShrinkAndDestroy(GameObject fireObject)
    {
        if (fireObject == null) yield break;
        
        Vector3 originalScale = fireObject.transform.localScale;
        float elapsedTime = 0f;
        
        if (showDebugLog)
            Debug.Log($"Fire 오브젝트 {fireObject.name} 크기 축소 시작!");
        
        while (elapsedTime < shrinkDuration && fireObject != null && isShrinking)
        {
            elapsedTime += Time.deltaTime;
            
            float shrinkProgress = Mathf.SmoothStep(0f, 1f, elapsedTime / shrinkDuration);
            Vector3 newScale = Vector3.Lerp(originalScale, Vector3.zero, shrinkProgress);
            
            fireObject.transform.localScale = newScale;
            
            // 임계값 체크 최적화
            if (newScale.x <= destroyThreshold && newScale.y <= destroyThreshold)
            {
                if (showDebugLog)
                    Debug.Log($"Fire 오브젝트 {fireObject.name}이 임계값({destroyThreshold}) 이하로 축소되어 즉시 제거됩니다!");
                
                Destroy(fireObject);
                ResetState();
                yield break;
            }
            
            yield return null;
        }
        
        // 시간 초과 시 파괴
        if (fireObject != null)
        {
            if (showDebugLog)
                Debug.Log($"Fire 오브젝트 {fireObject.name}이 완전히 사라졌습니다!");
            Destroy(fireObject);
        }
        
        ResetState();
    }
    
    public void ManualDestroy()
    {
        if (showDebugLog)
            Debug.Log($"{gameObject.name}이 수동으로 파괴됩니다!");
        Destroy(gameObject);
    }
    
    public void SetShrinkDuration(float duration) => shrinkDuration = Mathf.Max(0.1f, duration);
    public void SetDestroyThreshold(float threshold) => destroyThreshold = Mathf.Clamp01(threshold);
    
    void OnDestroy()
    {
        if (shrinkCoroutine != null)
            StopCoroutine(shrinkCoroutine);
    }
}
