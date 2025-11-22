using UnityEngine;

public class FireRopeController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject fire;
    [SerializeField] private Rigidbody2D rock;

    private bool isUsed = false;

    private void Update()
    {
        if (isUsed)
            return;

        if (fire == null)
        {
            // 불 오브젝트 소멸 함.
            DropRock();
        }
    }

    private void DropRock()
    {
        rock.bodyType = RigidbodyType2D.Dynamic;
        isUsed = true;
    }
}
