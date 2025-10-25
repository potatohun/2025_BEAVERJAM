using UnityEngine;

public class TrapController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            Move.Singleton_Move.SetDead();
        }
    }
}
