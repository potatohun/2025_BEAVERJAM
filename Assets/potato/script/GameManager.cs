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
        FadeInOutController.instance.FadeOutIn();
        Invoke("Respawn", FadeInOutController.instance.GetPlayTime());
    }

    public void Respawn() {
        Move.Singleton_Move.transform.position = SavePointManager.instance.GetCurrentSavePoint().transform.position;
        Move.Singleton_Move.Respawn();
    }
}
