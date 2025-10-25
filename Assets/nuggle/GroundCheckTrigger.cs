using UnityEngine;

public class GroundCheckTrigger : MonoBehaviour
{
    private Move playerMove;
    private int groundCount = 0;
    
    public void Initialize(Move moveScript)
    {
        playerMove = moveScript;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ground 레이어인지 확인
        if (IsGroundLayer(other.gameObject))
        {
            groundCount++;
            if (groundCount == 1) // 처음 접촉할 때만 상태 변경
            {
                playerMove?.UpdateGroundedState(true);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        // Ground 레이어인지 확인
        if (IsGroundLayer(other.gameObject))
        {
            groundCount--;
            if (groundCount <= 0) // 모든 ground와 접촉이 끝났을 때
            {
                groundCount = 0; // 음수 방지
                playerMove?.UpdateGroundedState(false);
            }
        }
    }
    
    private bool IsGroundLayer(GameObject obj)
    {
        if (playerMove == null) return false;
        
        // LayerMask를 사용하여 ground 레이어인지 확인
        int objLayer = obj.layer;
        int groundLayerMask = playerMove.GroundLayerMask.value;
        
        return (groundLayerMask & (1 << objLayer)) != 0;
    }
}
