using UnityEngine;

public class SavePointController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int index = 0;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SavePointManager.instance.SetCurrentSavePoint(this);
        }
    }

    public int GetIndex()
    {
        return index;
    }
}
