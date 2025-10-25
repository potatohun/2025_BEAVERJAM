using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Unity.Cinemachine;

public class TalkController : MonoBehaviour
{
    public static TalkController instance;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float textOffset = 2f;
    [SerializeField] private float talkSoundDelay = 0.1f;

    [Header("Talk Targets")]
    public GameObject talkTarget_player;
    public GameObject talkTarge_npc;

    [Header("Talk UI Object")]
    public RectTransform ui_talk;
    public TextMeshProUGUI ui_text_talk;

    [Header("Status")]
    [SerializeField] private bool isPlaying = false;

    private TalkData currentTalkData;
    private GameObject currentTalkTarget;
    private Tween talkTextTween;

    private float talkSoundTime = 0f;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        talkTarget_player = GameObject.FindWithTag("Player");
    }

    void Update() {
        if(Input.anyKeyDown) {
            if(currentTalkData == null) {
                return;
            }

            NextTalk();
        }

        // 현재 타겟이 있으면 타겟 위치로 이동
        if(currentTalkTarget != null)
        {
            ui_talk.anchoredPosition = GetTargetPositionToCanvas(currentTalkTarget);
        }
    }

    public void StartTalk(string group_id, GameObject npc) {
        talkTarge_npc = npc;

        // 카메라 줌인
        CameraManager.instance.ZoomInToTarget(talkTarget_player, talkTarge_npc);

        TalkData talkData = TalkDataLoader.instance.GetTalkDataByGroup(group_id);
        if (talkData == null) {
            Debug.LogWarning($"그룹 ID '{group_id}'를 찾을 수 없습니다.");
            return;
        }

        // UI 표시
        ui_talk.gameObject.SetActive(true);

        // 플레이어 제어
        Move.Singleton_Move.StartDialogue();

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
                currentTalkTarget = talkTarget_player;
                break;
            case "npc":
                currentTalkTarget = talkTarge_npc;
                break;
        }

        // 텍스트 표시
        int textLength = currentTalkData.talk.Length;
        
        // "<" 와 ">" 사이의 글자 수를 length에서 빼기
        string text = currentTalkData.talk;
        int bracketCount = 0;
        bool isOpenTag = false;
        for (int i = 0; i < text.Length; i++) {
            if (text[i] == '<') {
                isOpenTag = true;
                bracketCount++; // '<' 문자도 카운트
            } else if (text[i] == '>') {
                isOpenTag = false;
                bracketCount++; // '>' 문자도 카운트
            } else if (isOpenTag) {
                bracketCount++; // 태그 안의 글자들도 카운트
            }
        }
        textLength -= bracketCount;

        // 텍스트 연출 표시
        talkTextTween = ui_text_talk.DOText(currentTalkData.talk, textLength * textSpeed).OnUpdate(() => {
            if(Time.time - talkSoundTime > talkSoundDelay) {
                SoundManager.instance.PlaySound("talk");
                talkSoundTime = Time.time;
            }
        }).OnComplete(() => {
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
        // 대화 종료 시, 캐릭터 스킬 해금
        if(currentTalkTarget != null && currentTalkTarget.GetComponentInParent<NpcController>() != null) {
            Debug.Log("대화 종료 시, 캐릭터 스킬 해금");
            NpcController npcController = currentTalkTarget.GetComponentInParent<NpcController>();
            if(npcController.GetCanUnlock()) {
                Debug.Log("캐릭터 스킬 해금");
                FriendManager.FM.UnlockSkill(npcController.GetSkill());
            }
        } else {
            Debug.Log("대화 종료 시, 캐릭터 스킬 해금 불가");   
        }
            

        currentTalkData = null;
        currentTalkTarget = null;
        ui_talk.gameObject.SetActive(false);
        CameraManager.instance.ZoomOut();

        // 플레이어 제어
        Move.Singleton_Move.EndDialogue();
    }

    public Vector2 GetTargetPositionToCanvas(GameObject target)
    {
        Camera playerCamera = CameraManager.instance.GetMainCamera();
        
        // 타겟 위치에 오프셋 적용
        Vector3 worldPosition = target.transform.position + new Vector3(0, textOffset, 0);
        
        // Canvas 컴포넌트 찾기
        Canvas canvas = ui_talk.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        // Cinemachine 카메라를 위한 좌표 변환
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);
        
        // 스크린 좌표를 Canvas 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : playerCamera,
            out localPoint
        );
        
        return localPoint;
    }

    public Vector2 GetPlayerPositionToCanvas(GameObject player)
    {
        Camera playerCamera = CameraManager.instance.GetMainCamera();
        
        // Canvas 컴포넌트 찾기
        Canvas canvas = ui_talk.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        // Cinemachine 카메라를 위한 좌표 변환
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(player.transform.position);
        
        // 스크린 좌표를 Canvas 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : playerCamera,
            out localPoint
        );
        
        return localPoint;
    }

    public bool IsTalking() {
        return currentTalkData != null;
    }
}
