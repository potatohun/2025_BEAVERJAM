using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugManager : MonoBehaviour
{
    private void Update() {
        if(Input.GetKeyDown(KeyCode.F5)) {
            SceneManager.LoadScene("Intro");
        }
    }
}
