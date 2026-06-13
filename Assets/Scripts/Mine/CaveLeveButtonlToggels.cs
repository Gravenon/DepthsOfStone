using UnityEngine;
using UnityEngine.InputSystem;

public class CaveLeveButtonlToggels : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup mineDificultCanvasGroup;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference cancelAction;

    private bool _playerInRange;
    private bool _isOpen;

    private void Awake()
    {
        // Always start hidden and non-blocking
        Hide();
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.performed += OnInteract;
        if (cancelAction != null) cancelAction.action.performed    += OnCancel;
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteract;
        if (cancelAction  != null) cancelAction.action.performed    -= OnCancel;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _playerInRange = false;
            if (_isOpen) Hide();
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_isOpen) 
        { 
            Hide(); 
            return; 
        }
        if (_playerInRange) Show();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (_isOpen) Hide();
    }

    public void CloseDificultCanvas() => Hide();

    private void Show()
    {
        _isOpen = true;
        Time.timeScale = 0f;
        SetCanvas(true);
    }

    private void Hide()
    {
        _isOpen = false;
        Time.timeScale = 1f;
        SetCanvas(false);
    }

    private void SetCanvas(bool visible)
    {
        if (mineDificultCanvasGroup == null) return;
        mineDificultCanvasGroup.alpha = visible ? 1f : 0f;
        mineDificultCanvasGroup.interactable = visible;
        mineDificultCanvasGroup.blocksRaycasts = visible;
    }
}
