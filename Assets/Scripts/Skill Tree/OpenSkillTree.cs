using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SkillTreeType
{
    Combat,
    Magic
}

public class OpenSkillTree : MonoBehaviour
{
    public static OpenSkillTree currentSkillTreeNPC;

    public Animator anim;

    [Header("UI References")]
    [SerializeField] private CanvasGroup skillTreeCanvasGroup;
    [SerializeField] private GameObject combatTree;
    [SerializeField] private GameObject magicTree;
    
    [Header("Skill Tree Type")]
    [SerializeField] private SkillTreeType treeType = SkillTreeType.Combat;
    
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference cancelAction;

    public static event Action<bool> OnSkillTreeOpenClose;
    private bool playerInRange;
    private bool isSkillTreeOpen;
    private bool _hasNpcTalk;
    private NPC_Talk _npcTalk;

    private void Start()
    {
        HideSkillTree();
        _npcTalk = GetComponent<NPC_Talk>();
        _hasNpcTalk = _npcTalk != null;
    }

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.performed += OnInteract;
        if (cancelAction != null)
            cancelAction.action.performed += OnCancel;

        DialogueManager.OnDialogueEnd += OnDialogueEnded;
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteract;
        if (cancelAction != null)
            cancelAction.action.performed -= OnCancel;

        DialogueManager.OnDialogueEnd -= OnDialogueEnded;
    }

    // Auto-open skill tree after dialogue ends if player is still in range
    private void OnDialogueEnded()
    {
        if (playerInRange && !isSkillTreeOpen)
            ShowSkillTree();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        var dm = GameManager.Instance?.DialogueManager;

        // Dialogue is active — NPC_Talk handles E, we do nothing
        if (dm != null && dm.isDialogueActive)
            return;

        if (_hasNpcTalk)
        {
            // Close skill tree with E
            if (isSkillTreeOpen)
            {
                HideSkillTree();
                return;
            }

            if (!playerInRange) return;

            // If NPC still has a valid conversation — let NPC_Talk handle E (don't open skill tree)
            if (_npcTalk != null && _npcTalk.converstations.Exists(c => c != null && c.IsConditionsMet()))
                return;

            // No more dialogue — open skill tree directly
            ShowSkillTree();
            return;
        }

        // Pure skill tree NPC (no NPC_Talk)
        if (isSkillTreeOpen) { HideSkillTree(); return; }
        if (playerInRange) ShowSkillTree();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (!isSkillTreeOpen)
            return;
        
        HideSkillTree();
    }

    private void ShowSkillTree()
    {
        Time.timeScale = 0;
        currentSkillTreeNPC = this;
        isSkillTreeOpen = true;
        OnSkillTreeOpenClose?.Invoke(true);
        
        skillTreeCanvasGroup.alpha = 1;
        skillTreeCanvasGroup.blocksRaycasts = true;
        skillTreeCanvasGroup.interactable = true;
        
        ActivateCorrectTree();
    }

    private void HideSkillTree()
    {
        Time.timeScale = 1;
        isSkillTreeOpen = false;
        currentSkillTreeNPC = null;
        OnSkillTreeOpenClose?.Invoke(false);
        
        skillTreeCanvasGroup.alpha = 0;
        skillTreeCanvasGroup.blocksRaycasts = false;
        skillTreeCanvasGroup.interactable = false;
        
        DeactivateAllTrees();
    }

    private void ActivateCorrectTree()
    {
        DeactivateAllTrees();
        
        switch (treeType)
        {
            case SkillTreeType.Combat:
                if (combatTree != null)
                    combatTree.SetActive(true);
                break;
            case SkillTreeType.Magic:
                if (magicTree != null)
                    magicTree.SetActive(true);
                break;
        }
    }

    private void DeactivateAllTrees()
    {
        if (combatTree != null)
            combatTree.SetActive(false);
        if (magicTree != null)
            magicTree.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null)
                anim.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null)
                anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
