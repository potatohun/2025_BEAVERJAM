using UnityEngine;

public class TrapControllerV2 : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            Move.Singleton_Move.SetDead();
        }
    }
}
