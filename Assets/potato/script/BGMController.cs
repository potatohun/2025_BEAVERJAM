using UnityEngine;

public class BGMController : MonoBehaviour
{
    public static BGMController instance;

    public GameObject bgm1;
    public GameObject bgm2;
    public GameObject bgm3;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        PlayBGM(1);
    }

    public void PlayBGM(int bgmIndex) {
        switch(bgmIndex) {
            case 1:
                bgm1.SetActive(true);
                bgm2.SetActive(false);
                bgm3.SetActive(false);
                break;
            case 2:
                bgm2.SetActive(true);
                bgm1.SetActive(false);
                bgm3.SetActive(false);
                break;
            case 3:
                bgm3.SetActive(true);
                bgm1.SetActive(false);
                bgm2.SetActive(false);
                break;
        }
    }
}
