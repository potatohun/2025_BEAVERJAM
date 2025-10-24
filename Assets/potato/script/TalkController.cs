using UnityEngine;

public class TalkController : MonoBehaviour
{
    public static TalkController instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
