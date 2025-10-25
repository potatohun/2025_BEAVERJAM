using UnityEngine;

public class SavePointController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            SavePointManager.instance.SetCurrentSavePoint(this.gameObject);
        }
    }
}
