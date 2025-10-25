using UnityEngine;

public class NpcController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isTalkable = true;
    [SerializeField] private bool reUseable = false;
    [SerializeField] private string group_id;

    [Header("Text Bubble")]
    [SerializeField] private GameObject textBubble;

    private void OnTriggerEnter2D(Collider2D other) {
        // 대화 가능 상태가 아니면 대화 불가 (이미 대화 했음)
        if(isTalkable == false) {
            return;
        }

        if(other.gameObject.tag == "Player") {
            // npc 다른 npc와 대화 중이면 대화 불가
            if(TalkController.instance.IsTalking())
                return;

            // 대화 시작
            TalkController.instance.StartTalk(group_id, textBubble);
            if(reUseable == false)
                isTalkable = false;
        }
    }
}
