using UnityEngine;
using DG.Tweening;

public class FadeInOutController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("UI Object")]
    public RectTransform ui_image_fade;

    private void Start() {
        FadeIn();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            FadeIn();
        }

        if(Input.GetKeyDown(KeyCode.G)) {
            FadeOut();
        }
    }
    public void FadeIn() {
        ui_image_fade.gameObject.SetActive(true);
        ui_image_fade.localScale = new Vector3(1, 1, 1);
        ui_image_fade.DOScale(100, fadeDuration).OnComplete(() => {
            ui_image_fade.gameObject.SetActive(false);
        });
    }

    public void FadeOut() {
        ui_image_fade.gameObject.SetActive(true);
        ui_image_fade.localScale = new Vector3(100, 100, 100);
        ui_image_fade.DOScale(1, fadeDuration).OnComplete(() => {
            ui_image_fade.gameObject.SetActive(false);
        });
    }
}
