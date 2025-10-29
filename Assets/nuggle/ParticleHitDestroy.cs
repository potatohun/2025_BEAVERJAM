using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ParticleHitDestroy : MonoBehaviour
{
    private int fireLayer;
    private int pondLayer;
    void Awake()
    {
        // 레이어 캐싱으로 성능 최적화
        fireLayer = LayerMask.NameToLayer("Fire");
        pondLayer = LayerMask.NameToLayer("Pond");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //// 캐싱된 레이어로 빠른 비교
        if (other.gameObject.layer == fireLayer)
        {

            other.GetComponent<FireDestroy>()?.SetisShrinking(true);
        }
        else if (other.gameObject.layer == pondLayer)
        {
            other.GetComponent<PondController>()?.FillPond();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == fireLayer)
        {
            other.GetComponent<FireDestroy>()?.SetisShrinking(false);
        }
    }


}
