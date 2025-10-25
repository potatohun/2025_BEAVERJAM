using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {

    }

    public void GameOver() {
        FadeInOutController.instance.FadeOut();
        Invoke("Restart", FadeInOutController.instance.GetPlayTime());
    }

    public void Restart() {
        Move.Singleton_Move.Respawn();
        Invoke("Respawn", FadeInOutController.instance.GetPlayTime());
    }

    public void Respawn() {
        FadeInOutController.instance.FadeIn();
    }
}
