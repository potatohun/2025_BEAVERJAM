using UnityEngine;
using System.Collections.Generic;

public class SavePointManager : MonoBehaviour
{
    public static SavePointManager instance;

    [Header("Save Point Object")]
    [SerializeField] private List<SavePointController> _savePointList;
    [SerializeField] private SavePointController _currentSavePoint;
    
    [SerializeField] private SavePointController _firstSavePoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 태그가 "SavePoint"인 모든 오브젝트를 찾아서 savePointObject에 추가
        foreach (GameObject savePoint in GameObject.FindGameObjectsWithTag("SavePoint"))
        {
            // 세이브 포인트 저장
            SavePointController savePointController = savePoint.GetComponent<SavePointController>();
            _savePointList.Add(savePointController);
            
            // 첫번째 세이브 포인트 지점 설정
            if (savePointController.GetIndex() == 0)
                _firstSavePoint = savePointController;
        }
    }

    public void SetCurrentSavePoint(SavePointController savePoint)
    {
        // 처음 세이브 포인트 지점 설정
        if (_currentSavePoint == null)
        {
            _currentSavePoint = savePoint;
            NotiManager.instance.ShowNoti(4);
            return;
        }
        
        // Index가 더 큰 세이브 포인트 지점 설정
        if(_currentSavePoint.GetIndex() < savePoint.GetIndex())
        {
            _currentSavePoint = savePoint;
            NotiManager.instance.ShowNoti(4);
        }
    }

    public SavePointController GetCurrentSavePoint()
    {
        if (_currentSavePoint == null)
            return _firstSavePoint;
        else
            return _currentSavePoint;
    }
}
