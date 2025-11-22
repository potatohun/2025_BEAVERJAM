using UnityEngine;
using System.Collections.Generic;
public class PortalController : MonoBehaviour
{
    [Header("MapController")]
    [SerializeField] private MapController currentMap;
    [SerializeField] private MapController nextMap;

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            Teleport();
        }
    }
    public void Teleport() {
        FadeInOutController.instance.FadeOutIn();
        
        Invoke("MovePlayerToNextMap", FadeInOutController.instance.GetPlayTime());
    }

    public void MovePlayerToNextMap() {
        // 플레이어를 다음 맵으로 이동
        Move.Singleton_Move.transform.position = nextMap.GetStartPoint().position;

        // 카메라 컨피너 설정
        CameraManager.instance.SetPlayerConfiner(nextMap.GetMapCollider());

        // 카메라 팔로우
        CameraManager.instance.ForceFollowPlayer();

        // 이전 맵 초기화
        //currentMap.ClearMap();

        // 다음 맵 초기화
        nextMap.InitMap();

        // BGM 변경
        BGMController.instance.PlayBGM(nextMap.GetMapIndex());

        // 알림 표시
        NotiManager.instance.ShowNoti(nextMap.GetMapIndex());
    }
}
