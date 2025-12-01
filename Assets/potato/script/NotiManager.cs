using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class NotiManager : MonoBehaviour
{
    public static NotiManager instance;

    [Header("Settings")]
    [SerializeField] private float notiDuration = 3f;
    [SerializeField] private float notiDelay = 1f;

    [Header("Noti Object")]
    public List<Image> notiList;

    private List<Tween> notiTweenList = new List<Tween>();

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

        for (int i = 0; i < notiList.Count; i++)
        {
            notiTweenList.Add(null);
        }
    }

    private void Start()
    {
        ShowNoti(1);
    }

    public void ShowNoti(int index)
    {
        if (notiTweenList[index] != null)
        {
            notiTweenList[index].Complete();
            notiTweenList[index] = null;
        }

        notiList[index].gameObject.SetActive(true);
        notiList[index].color = new Color(1f, 1f, 1f, 1f);
        notiTweenList[index] = notiList[index].DOFade(0f, notiDuration).SetEase(Ease.OutQuad).SetDelay(notiDelay).OnComplete(() => {
            notiList[index].gameObject.SetActive(false);
        });
    }
}
