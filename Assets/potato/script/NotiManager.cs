using UnityEngine;
using System.Collections.Generic;

public class NotiManager : MonoBehaviour
{
    public static NotiManager instance;

    public List<GameObject> notiList;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        ShowNoti(1);
    }
    
    public void ShowNoti(int index) {
        notiList[index].SetActive(true);
    }
}
