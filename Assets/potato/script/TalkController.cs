using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TalkController : MonoBehaviour
{
    public static TalkController instance;

    [Header("Talk Targets")]
    public GameObject talkTarget_player;
    public GameObject talkTarge_npc;

    [Header("Talk UI Object")]
    public RectTransform ui_talk;
    public TextMeshProUGUI ui_text_talk;

    [Header("Status")]
    public bool isPlaying = false;

    private TalkData currentTalkData;
    private Tween talkTextTween;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            StartTalk("start");
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            NextTalk();
        }
    }

    public void StartTalk(string group_id) {
        TalkData talkData = TalkDataLoader.instance.GetTalkDataByGroup(group_id);
        if (talkData == null) {
            Debug.LogWarning($"그룹 ID '{group_id}'를 찾을 수 없습니다.");
            return;
        }

        // UI 표시
        ui_talk.gameObject.SetActive(true);

        ShowTalk(talkData);
    }

    public void ShowTalk(TalkData talkData) {
         // 텍스트 초기화
        ui_text_talk.text = "";

        // 현재 대화 데이터 설정
        currentTalkData = talkData;
        isPlaying = true;

        // 타겟 포지션 설정
        switch(talkData.start) {
            case "player":
                Vector2 playerPosition = GetTargetPositionToCanvas(talkTarget_player);
                ui_talk.anchoredPosition = playerPosition;
                break;
            case "npc":
                Vector2 npcPosition = GetTargetPositionToCanvas(talkTarge_npc);
                ui_talk.anchoredPosition = npcPosition;
                break;
        }

        // 텍스트 표시
        int textLength = currentTalkData.talk.Length;
        talkTextTween = ui_text_talk.DOText(currentTalkData.talk, textLength * 0.1f).OnComplete(() => {
            isPlaying = false;
            talkTextTween = null;
        });
    }

    public void NextTalk() {
        // 대화 애니메이션 존재 시, 애니메이션 종료
        if(talkTextTween != null) {
            talkTextTween.Complete();
            return;
        }
        
        // 다음 대화 데이터 조회
        if(currentTalkData.next_talk_id == null || currentTalkData.next_talk_id == "end") {
            // 다음 대화 데이터가 없음
            EndTalk();
        } else {
            // 다음 대화 데이터 조회
            TalkData talkData = TalkDataLoader.instance.GetTalkData(currentTalkData.next_talk_id);
            if(talkData == null) {
                Debug.LogWarning("다음 대화가 없습니다.");
                EndTalk();
            }

            // 다음 대화 표시
            ShowTalk(talkData);
        }
    }

    public void EndTalk() {
        ui_talk.gameObject.SetActive(false);
    }

    public Vector2 GetTargetPositionToCanvas(GameObject target)
    {
        // Orthographic 카메라의 경우 직접 월드 좌표를 사용
        Vector3 worldPosition = target.transform.position + new Vector3(0, 2, 0);
        
        // Orthographic 카메라의 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // Canvas 컴포넌트 찾기
        Canvas canvas = ui_talk.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        // 스크린 좌표를 Canvas 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out localPoint
        );
        
        // Orthographic 카메라의 경우 추가 오프셋 적용
        if (Camera.main.orthographic)
        {
            // 카메라의 orthographicSize를 고려한 스케일링
            float orthoSize = Camera.main.orthographicSize;
            float aspectRatio = (float)Screen.width / Screen.height;
            
            // 월드 좌표를 직접 Canvas 좌표로 변환
            Vector2 worldToCanvas = new Vector2(
                (worldPosition.x / (orthoSize * aspectRatio)) * canvasRect.rect.width * 0.5f,
                (worldPosition.y / orthoSize) * canvasRect.rect.height * 0.5f
            );
            
            return worldToCanvas;
        }
        
        return localPoint;
    }
}
