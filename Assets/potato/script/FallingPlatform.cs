using UnityEngine;
using DG.Tweening;

public class FallingPlatform : MonoBehaviour
{
    private Tween moveDownTween;
    private Vector3 originalPosition;

    private void Awake() {
        originalPosition = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            MoveDown();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            MoveUp();
        }
    }

    private void MoveDown() 
    {
        if(moveDownTween != null) {
            moveDownTween.Kill();
            moveDownTween = null;
        }

        moveDownTween = transform.DOMoveY(originalPosition.y - 10f, 1f).SetEase(Ease.InSine);
    }
    
    private void MoveUp() {
        if(moveDownTween != null) {
            moveDownTween.Kill();
            moveDownTween = null;
        }

        moveDownTween = transform.DOMoveY(originalPosition.y, 1f).SetEase(Ease.InSine);
    }
}
