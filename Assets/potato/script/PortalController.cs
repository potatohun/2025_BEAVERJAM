using UnityEngine;
using System.Collections.Generic;
public class PortalController : MonoBehaviour
{
    public Transform nextMapPoint;
    public BoxCollider2D nextMapCollider;

    public List<GameObject> ballList;

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            Teleport();
        }
    }
    public void Teleport() {
        foreach(GameObject ball in ballList) {
            ball.SetActive(false);
        }

        FadeInOutController.instance.FadeOutIn();
        
        Invoke("MovePlayerToNextMap", FadeInOutController.instance.GetPlayTime());
    }

    public void MovePlayerToNextMap() {
        CameraManager.instance.SetPlayerConfiner(nextMapCollider);
        Move.Singleton_Move.transform.position = nextMapPoint.position;
        CameraManager.instance.ForceFollowPlayer();
        BGMController.instance.PlayBGM(2);
        NotiManager.instance.ShowNoti(2);
    }
}
