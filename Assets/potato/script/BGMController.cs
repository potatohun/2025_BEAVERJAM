using UnityEngine;

public class BGMController : MonoBehaviour
{
    public static BGMController instance;

    public GameObject bgm0;
    public GameObject bgm1;
    public GameObject bgm2;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        PlayBGM(0);
    }

    public void PlayBGM(int bgmIndex) {
        switch(bgmIndex) {
            case 0:
                bgm0.SetActive(true);
                bgm1.SetActive(false);
                bgm2.SetActive(false);
                break;
            case 1:
                bgm0.SetActive(false);
                bgm1.SetActive(true);
                bgm2.SetActive(false);
                break;
            case 2:
                bgm0.SetActive(false);
                bgm1.SetActive(false);
                bgm2.SetActive(true);
                break;
        }
    }
}
