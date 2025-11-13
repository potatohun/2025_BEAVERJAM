using UnityEngine;
using DG.Tweening;

public class FadeInOutController : MonoBehaviour
{
    public static FadeInOutController instance;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeDelay = 2f;
    [SerializeField] private float targetMinScale = 0.1f;
    [SerializeField] private float targetMaxScale = 200f;
    [SerializeField] private Vector2 holeOffset = new Vector2(0, 0);

    [Header("UI Object")]
    public GameObject backgroundCanvas;
    public GameObject holeCanvas;
    public RectTransform holeRectTransform;

    private GameObject player;
    private Tween fadeTween;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        
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

        holeRectTransform.localScale = new Vector3(targetMinScale, targetMinScale, targetMinScale);
        fadeTween = holeRectTransform.DOScale(targetMaxScale, fadeDuration).OnUpdate(() => {
            holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player, holeOffset);
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
        
        holeRectTransform.localScale = new Vector3(targetMaxScale, targetMaxScale, targetMaxScale);
        holeRectTransform.DOScale(1, fadeDuration).OnUpdate(() => {
            holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player, holeOffset);
        }).OnComplete(() => {
            // 배경 캔버스와 구멍 캔버스 비활성화
            backgroundCanvas.gameObject.SetActive(false);
            holeCanvas.gameObject.SetActive(false);
            fadeTween = null;
        });
    }

    public float GetPlayTime() {
        return fadeDuration;
    }
    
    public void FadeOutIn() {
        // 배경 캔버스와 구멍 캔버스 활성화
        backgroundCanvas.gameObject.SetActive(true);
        holeCanvas.gameObject.SetActive(true);

        if(fadeTween != null) {
            fadeTween.Complete();
            fadeTween = null;
        }
        
        holeRectTransform.localScale = new Vector3(targetMaxScale, targetMaxScale, targetMaxScale);
        holeRectTransform.DOScale(targetMinScale, fadeDuration).OnUpdate(() => {
            holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player, holeOffset);
        }).OnComplete(() => {
            holeRectTransform.localScale = new Vector3(targetMinScale, targetMinScale, targetMinScale);
            fadeTween = holeRectTransform.DOScale(targetMaxScale, fadeDuration).OnUpdate(() => {
                holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player, holeOffset);
            }).SetDelay(fadeDelay).OnComplete(() => {
                // 배경 캔버스와 구멍 캔버스 비활성화
                backgroundCanvas.gameObject.SetActive(false);
                holeCanvas.gameObject.SetActive(false);
                fadeTween = null;
            });
        });
    }
}
