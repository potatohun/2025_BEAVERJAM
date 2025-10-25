using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SquareMovePlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waitTime = 1f;
    
    [Header("Move Points")]
    [SerializeField] private List<Vector3> movePoints = new List<Vector3>();
    
    [Header("Debug")]
    [SerializeField] private bool loopPath = true;
    
    private int currentPointIndex = 0;
    private Coroutine moveCoroutine;

    private void Start() {
        if(movePoints.Count > 0) {
            // 첫 번째 포인트로 이동
            transform.position = movePoints[0];
            moveCoroutine = StartCoroutine(MovePlatform());
        }
    }
    
    private IEnumerator MovePlatform() {
        while(true) {
            // 현재 목표 지점까지 이동
            if(movePoints.Count > 0) {
                Vector3 targetPoint = movePoints[currentPointIndex];
                
                // 거리 계산
                float distance = Vector3.Distance(transform.position, targetPoint);
                float duration = distance / moveSpeed;
                
                // DOTween으로 부드럽게 이동
                yield return transform.DOMove(targetPoint, duration).WaitForCompletion();
                
                // 목표 지점에 도달하면 잠시 대기
                yield return new WaitForSeconds(waitTime);
                
                // 다음 지점 인덱스 계산
                currentPointIndex++;
                
                if(loopPath) {
                    // 순환: 인덱스가 마지막을 넘으면 0으로
                    currentPointIndex %= movePoints.Count;
                } else {
                    // 왕복: 끝에 도달하면 역순으로
                    if(currentPointIndex >= movePoints.Count) {
                        currentPointIndex = movePoints.Count - 2;
                    } else if(currentPointIndex < 0) {
                        currentPointIndex = 1;
                    }
                }
            } else {
                yield return null;
            }
        }
    }
    
    private void OnDrawGizmos() {
        // 에디터에서 이동 경로 시각화
        if(movePoints == null || movePoints.Count == 0) return;
        
        Gizmos.color = Color.yellow;
        
        for(int i = 0; i < movePoints.Count; i++) {
            // 이동 지점 표시
            Gizmos.DrawWireSphere(movePoints[i], 0.2f);
            
            // 다음 지점까지 선 그리기
            int nextIndex = (i + 1) % movePoints.Count;
            Gizmos.DrawLine(movePoints[i], movePoints[nextIndex]);
        }
    }
}
