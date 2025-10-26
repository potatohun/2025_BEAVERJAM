using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera zoomInCamera;

    [SerializeField] private CinemachineConfiner2D playerConfiner;
    [SerializeField] private CinemachineConfiner2D zoomInConfiner;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public Camera GetMainCamera() {
        return mainCamera;
    }

    public CinemachineCamera GetPlayerCamera() {
        return playerCamera;
    }

    public CinemachineCamera GetZoomInCamera() {
        return zoomInCamera;
    }

    public void ZoomInToTarget(GameObject target1, GameObject target2) {
        // 두 타겟의 평균 위치에 zoomInCamera를 위치
        Vector3 averagePosition = (target1.transform.position + target2.transform.position) / 2;
        zoomInCamera.transform.position = new Vector3(averagePosition.x, averagePosition.y, zoomInCamera.transform.position.z);

        zoomInCamera.gameObject.SetActive(true);
    }

    public void ZoomOut() {
        zoomInCamera.gameObject.SetActive(false);
    }

    public void SetPlayerConfiner(BoxCollider2D collider) {
        if(playerConfiner != null) {
            playerConfiner.BoundingShape2D = collider;
        }
        if(zoomInConfiner != null) {
            zoomInConfiner.BoundingShape2D = collider;
        }
    } 
}
