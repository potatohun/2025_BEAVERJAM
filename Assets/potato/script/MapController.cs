using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int mapIndex;

    [Header("Objects")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private List<GameObject> mapObjects;
    [SerializeField] private BoxCollider2D mapCollider;

    public void InitMap()
    {
        foreach (GameObject obj in mapObjects)
        {
            obj.SetActive(true);
        }
    }

    public void ClearMap()
    {
        foreach (GameObject obj in mapObjects)
        {
            obj.SetActive(false);
        }
    }

    public BoxCollider2D GetMapCollider()
    {
        return mapCollider;
    }

    public int GetMapIndex()
    {
        return mapIndex;
    }
    
    public Transform GetStartPoint()
    {
        return startPoint;
    }
}
