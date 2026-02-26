using UnityEngine;
using UnityEngine.InputSystem;

public class OreMining : MonoBehaviour
{
    public float interactRange = 1.5f;
    public LayerMask resourceLayer;

    public void Fire(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (Pointer.current == null || Camera.main == null)
        {
            return;
        }

        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        TryMineUnderPointer(pointerPosition);
    }

    void TryMineUnderPointer(Vector2 pointerPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(pointerPosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition, resourceLayer);
        if (hit != null)
        {
            float distance = Vector2.Distance(hit.ClosestPoint(transform.position), transform.position);

            if (distance <= interactRange)
            {
                Ore ore = hit.GetComponent<Ore>();
                if (ore == null)
                {
                    ore = hit.gameObject.AddComponent<Ore>();
                }
                ore.Mine();
            }
        }

    }
}
