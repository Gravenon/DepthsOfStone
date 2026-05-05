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


    private void Start()
    {
        HideSkillTree();
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteract;
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed += OnCancel;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed -= OnCancel;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isSkillTreeOpen)
        {
            HideSkillTree();
            return;
        }

        if (!playerInRange)
            return;
        
        ShowSkillTree();
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
