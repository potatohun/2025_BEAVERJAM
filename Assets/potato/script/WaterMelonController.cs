using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterMelonController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float despawnTime = 10f;

    [Header("Object")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject waterMelonPrefab;

    private Queue<GameObject> watermelonPool = new Queue<GameObject>();
    private List<GameObject> activeWatermelons = new List<GameObject>();

    private void Start() {
        InitializePool();
        StartCoroutine(SpawnWaterMelon());
    }

    private void InitializePool() {
        // 풀에 오브젝트 미리 생성
        for (int i = 0; i < poolSize; i++) {
            GameObject watermelon = Instantiate(waterMelonPrefab);
            watermelon.SetActive(false);
            watermelonPool.Enqueue(watermelon);
        }
    }

    private GameObject GetFromPool() {
        // 풀에서 사용 가능한 오브젝트 가져오기
        if (watermelonPool.Count > 0) {
            return watermelonPool.Dequeue();
        } else {
            // 풀이 비어있으면 새로 생성
            GameObject watermelon = Instantiate(waterMelonPrefab);
            return watermelon;
        }
    }

    public void ReturnToPool(GameObject watermelon) {
        // 오브젝트를 풀로 반환
        watermelon.SetActive(false);
        watermelonPool.Enqueue(watermelon);
        activeWatermelons.Remove(watermelon);
    }

    public IEnumerator SpawnWaterMelon() {
        while(true) {
            yield return new WaitForSeconds(spawnInterval);
            
            // 스폰할 지점 선택
            if(spawnPoints.Length > 0) {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                
                // 풀에서 수박 가져오기
                GameObject watermelon = GetFromPool();
                activeWatermelons.Add(watermelon);
                
                // 스폰 지점으로 이동 및 활성화
                watermelon.transform.position = spawnPoint.position;
                watermelon.transform.rotation = spawnPoint.rotation;
                watermelon.SetActive(true);
                
                // 10초 후 자동으로 풀로 반환
                StartCoroutine(ReturnToPoolAfterDelay(watermelon, despawnTime));
            }
        }
    }
    
    private IEnumerator ReturnToPoolAfterDelay(GameObject watermelon, float delay) {
        yield return new WaitForSeconds(delay);
        
        // 수박이 아직 활성 상태라면 풀로 반환
        if(watermelon != null && watermelon.activeSelf) {
            ReturnToPool(watermelon);
        }
    }
}
