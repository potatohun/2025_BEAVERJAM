using UnityEngine;
using System.Collections.Generic;

public class SavePointManager : MonoBehaviour
{
    public static SavePointManager instance;

    [Header("Save Point Object")]
    [SerializeField] private List<GameObject> savePointObject;
    [SerializeField] private GameObject currentSavePoint;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetCurrentSavePoint(GameObject savePoint) {
        if(savePointObject.Contains(savePoint) == false)
            return;

        // 첫 저장 지점 설정
        if(currentSavePoint == null) {
            currentSavePoint = savePoint;
        } else {
            // 현재 저장 지점보다 뒤에 있는 저장 지점 설정
            if(savePointObject.IndexOf(savePoint) > savePointObject.IndexOf(currentSavePoint))
                currentSavePoint = savePoint;
        }
    }

    public GameObject GetCurrentSavePoint() {
        if(currentSavePoint == null)
            return savePointObject[0];
        else  
            return currentSavePoint;
    }
}
