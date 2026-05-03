using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OreMining : MonoBehaviour
{
    [Header("Mining Settings")]
    public float interactRange = 1.5f;
    public LayerMask resourceLayer;
    public float miningDuration = 2f; 
    
    [Header("Animation")]
    public Animator anim;
    
    private float miningTimer = 0f;
    private Ore currentOre = null;
    private bool isMousePressed = false;

    public Slider miningProgressSlider;


    public void Fire(InputAction.CallbackContext context)
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        if (context.started || context.performed)
        {
            isMousePressed = true;
            Vector2 pointerPosition = Mouse.current.position.ReadValue();
            TryStartMining(pointerPosition);
        }
        else if (context.canceled)
        {
            isMousePressed = false;
            StopMining();
        }
    }

    private void Update()
    {
        if (isMousePressed)
        {
            miningTimer += Time.deltaTime;

            miningProgressSlider.maxValue = miningDuration;
            miningProgressSlider.gameObject.SetActive(true);
            miningProgressSlider.value = miningTimer;
            
            if (miningTimer >= miningDuration)
            {
                CompleteMining();
            }
        }
    }

    void TryStartMining(Vector2 pointerPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(pointerPosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition, resourceLayer);
        if (hit != null)
        {
            float distance = Vector2.Distance(hit.ClosestPoint(transform.position), transform.position);

            if (distance <= interactRange)
            {
                Ore ore = hit.GetComponent<Ore>();
                if (ore != null)
                {
                    currentOre = ore;
                    miningTimer = 0f;
                    
                    anim.SetBool("isMining", true);
                }
            }
        }
    }

    void CompleteMining()
    {
        if (currentOre != null)
        {
            currentOre.Mine();
        }
        
        StopMining();
    }

    void StopMining()
    {
        miningTimer = 0f;
        currentOre = null;

        miningProgressSlider.gameObject.SetActive(false);
        miningProgressSlider.value = miningTimer;
        
        anim.SetBool("isMining", false);
    }
}
