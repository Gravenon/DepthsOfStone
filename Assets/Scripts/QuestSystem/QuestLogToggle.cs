using UnityEngine;
using UnityEngine.InputSystem;

public class QuestLogToggle : MonoBehaviour
{
    [SerializeField] private CanvasGroup questLogCanvasGroup;
    [SerializeField] private QuestLogUI questLogUI;
    [SerializeField] public InputActionReference toggleQuestLogAction;

    private bool _isOpen = false;

    private void Start()
    {
        SetVisible(false);
    }

    private void OnEnable()
    {
        if (toggleQuestLogAction != null)
            toggleQuestLogAction.action.performed += OnToggleQuestLog;
    }

    private void OnDisable()
    {
        if (toggleQuestLogAction != null)
            toggleQuestLogAction.action.performed -= OnToggleQuestLog;
    }

    private void OnToggleQuestLog(InputAction.CallbackContext context)
    {
        _isOpen = !_isOpen;
        SetVisible(_isOpen);

        // Close detail panel when the log closes
        if (!_isOpen && questLogUI != null && questLogUI)
            questLogUI.CloseDetail();
    }

    private void SetVisible(bool visible)
    {
        if (questLogCanvasGroup == null)
        {
            Debug.LogWarning("[QuestLogToggle] questLogCanvasGroup is not assigned!");
            return;
        }

        questLogCanvasGroup.alpha          = visible ? 1f : 0f;
        questLogCanvasGroup.interactable   = visible;
        questLogCanvasGroup.blocksRaycasts = visible;
    }

    public void OpenQuestLog()
    {
        _isOpen = true;
        SetVisible(true);
    }

    public void CloseQuestLog()
    {
        _isOpen = false;
        SetVisible(false);

        if (questLogUI != null && questLogUI)
            questLogUI.CloseDetail();
    }
}
