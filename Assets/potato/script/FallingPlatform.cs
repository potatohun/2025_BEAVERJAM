using UnityEngine;
using DG.Tweening;

public class FallingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallDistance = 10f;
    [SerializeField] private float fallTime = 1f;
    [SerializeField] private Ease fallEase = Ease.InSine;
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

        moveDownTween = transform.DOMoveY(originalPosition.y - fallDistance, fallTime).SetEase(fallEase);
    }
    
    private void MoveUp() {
        if(moveDownTween != null) {
            moveDownTween.Kill();
            moveDownTween = null;
        }

        moveDownTween = transform.DOMoveY(originalPosition.y, fallTime).SetEase(fallEase);
    }
}
