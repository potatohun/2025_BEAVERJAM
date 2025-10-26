using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OnClickStartButton(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            OnClickStartButton("InGame");
        }
    }
}
