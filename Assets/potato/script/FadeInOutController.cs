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
        if(fadeTween != null) {
            fadeTween.Kill();
            
            // 배경 캔버스와 구멍 캔버스 비활성화
            backgroundCanvas.gameObject.SetActive(false);
            holeCanvas.gameObject.SetActive(false);
            fadeTween = null;
        }

        // 배경 캔버스와 구멍 캔버스 활성화
        backgroundCanvas.gameObject.SetActive(true);
        holeCanvas.gameObject.SetActive(true);

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
        if(fadeTween != null) {
            fadeTween.Kill();
            
            // 배경 캔버스와 구멍 캔버스 비활성화
            backgroundCanvas.gameObject.SetActive(false);
            holeCanvas.gameObject.SetActive(false);
            fadeTween = null;
        }

        // 배경 캔버스와 구멍 캔버스 활성화
        backgroundCanvas.gameObject.SetActive(true);
        holeCanvas.gameObject.SetActive(true);
        
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
        if(fadeTween != null) {
            fadeTween.Kill();

            // 배경 캔버스와 구멍 캔버스 비활성화
            backgroundCanvas.gameObject.SetActive(false);
            holeCanvas.gameObject.SetActive(false);
            fadeTween = null;
        }

        // 플레이어 정지 및 무적
        Move.Singleton_Move.StopPlayer();

        // 배경 캔버스와 구멍 캔버스 활성화
        backgroundCanvas.gameObject.SetActive(true);
        holeCanvas.gameObject.SetActive(true);
        
        holeRectTransform.localScale = new Vector3(targetMaxScale, targetMaxScale, targetMaxScale);
        fadeTween = holeRectTransform.DOScale(targetMinScale, fadeDuration).OnUpdate(() => {
            holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player, holeOffset);
        }).OnComplete(() => {
            // 플레이어 재생
            Move.Singleton_Move.ResumePlayer();

            holeRectTransform.localScale = new Vector3(targetMinScale, targetMinScale, targetMinScale);
            fadeTween = holeRectTransform.DOScale(targetMaxScale, fadeDuration).OnUpdate(() => {
                holeRectTransform.anchoredPosition = TalkController.instance.GetPlayerPositionToCanvas(player, holeOffset);
            }).SetDelay(fadeDelay).OnComplete(() =>
            {
                // 배경 캔버스와 구멍 캔버스 비활성화
                backgroundCanvas.gameObject.SetActive(false);
                holeCanvas.gameObject.SetActive(false);
                fadeTween = null;
            });
        });
    }
}
