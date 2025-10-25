using UnityEngine;

public class ParticleHitDestroy : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        // 파티클이 자신에게 부딪혔을 때 제거
        Destroy(gameObject);
    }
}
