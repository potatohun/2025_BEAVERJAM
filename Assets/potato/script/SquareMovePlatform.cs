using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SquareMovePlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private Ease moveEase = Ease.Linear;
    
    [Header("Move Points")]
    [SerializeField] private List<Vector3> movePoints = new List<Vector3>();
    
    [Header("Debug")]
    [SerializeField] private bool loopPath = true;
    
    private int currentPointIndex = 0;
    private Coroutine moveCoroutine;
    private Vector3 startLocalPosition;

    private void Start() {
        if(movePoints.Count > 0) {
            // 시작 위치 저장 (현재 위치)
            startLocalPosition = transform.localPosition;
            moveCoroutine = StartCoroutine(MovePlatform());
        }
    }
    
    private IEnumerator MovePlatform() {
        while(true) {
            // 현재 목표 지점까지 이동
            if(movePoints.Count > 0) {
                // 시작 위치 + 상대 오프셋 = 목표 위치
                Vector3 targetPoint = startLocalPosition + movePoints[currentPointIndex];
                
                // 거리 계산 (로컬 좌표 기준)
                float distance = Vector3.Distance(transform.localPosition, targetPoint);
                float duration = distance / moveSpeed;
                
                // DOTween으로 부드럽게 이동 (로컬 좌표 기준)
                yield return transform.DOLocalMove(targetPoint, duration).SetEase(moveEase).WaitForCompletion();
                
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
        
        // 시작 위치 (에디터에서는 현재 위치를 시작 위치로 사용)
        Vector3 basePosition = Application.isPlaying ? startLocalPosition : transform.localPosition;
        
        // 부모가 있으면 부모의 월드 변환 행렬 사용, 없으면 월드 좌표 = 로컬 좌표
        Transform parentTransform = transform.parent;
        
        // 시작 위치를 월드 좌표로 변환
        Vector3 baseWorldPos = parentTransform != null 
            ? parentTransform.TransformPoint(basePosition) 
            : basePosition;
        
        Gizmos.color = Color.yellow;
        
        for(int i = 0; i < movePoints.Count; i++) {
            // 시작 위치 + 상대 오프셋 = 실제 목표 위치 (로컬 좌표)
            Vector3 targetLocalPos = basePosition + movePoints[i];
            // 로컬 좌표를 월드 좌표로 변환
            Vector3 worldPoint = parentTransform != null 
                ? parentTransform.TransformPoint(targetLocalPos) 
                : targetLocalPos;
            
            // 이동 지점 표시
            Gizmos.DrawWireSphere(worldPoint, 0.2f);
            
            // 다음 지점까지 선 그리기
            int nextIndex = (i + 1) % movePoints.Count;
            Vector3 nextTargetLocalPos = basePosition + movePoints[nextIndex];
            Vector3 nextWorldPoint = parentTransform != null 
                ? parentTransform.TransformPoint(nextTargetLocalPos) 
                : nextTargetLocalPos;
            Gizmos.DrawLine(worldPoint, nextWorldPoint);
        }
        
        // 시작 위치 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(baseWorldPos, 0.15f);
    }
}
