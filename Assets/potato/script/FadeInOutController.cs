using UnityEngine;
using DG.Tweening;

public class FadeInOutController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float targetScale = 200f;

    [Header("UI Object")]
    public GameObject backgroundCanvas;
    public GameObject holeCanvas;
    public RectTransform holeRectTransform;

    private GameObject player;
    private Tween fadeTween;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
    }

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
        // 배경 캔버스와 구멍 캔버스 활성화
        backgroundCanvas.gameObject.SetActive(true);
        holeCanvas.gameObject.SetActive(true);

        if(fadeTween != null) {
            fadeTween.Complete();
            fadeTween = null;
        }

        holeRectTransform.localScale = new Vector3(1, 1, 1);
        fadeTween = holeRectTransform.DOScale(targetScale, fadeDuration).OnUpdate(() => {
            holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player);
        }).OnComplete(() => {
            // 배경 캔버스와 구멍 캔버스 비활성화
            backgroundCanvas.gameObject.SetActive(false);
            holeCanvas.gameObject.SetActive(false);
            fadeTween = null;
        });
    }

    public void FadeOut() {
        // 배경 캔버스와 구멍 캔버스 활성화
        backgroundCanvas.gameObject.SetActive(true);
        holeCanvas.gameObject.SetActive(true);

        if(fadeTween != null) {
            fadeTween.Complete();
            fadeTween = null;
        }
        
        holeRectTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
        holeRectTransform.DOScale(1, fadeDuration).OnUpdate(() => {
            holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player);
        }).OnComplete(() => {
            // 배경 캔버스와 구멍 캔버스 비활성화
            backgroundCanvas.gameObject.SetActive(false);
            holeCanvas.gameObject.SetActive(false);
            fadeTween = null;
        });
    }
}
