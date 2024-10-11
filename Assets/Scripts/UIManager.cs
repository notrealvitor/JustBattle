using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [HideInInspector] public GameObject actionButtons;
    [HideInInspector] public GameObject actionsList;
    [HideInInspector] public Button attackButton;
    [HideInInspector] public Button skillsButton;
    [HideInInspector] public Button itemsButton;

    [HideInInspector] public GameObject playerHUD;  // Reference to the player's HUD
    [HideInInspector] public GameObject enemyHUD;   // Reference to the enemy's HUD

    private CombatManager combatManager;

    [HideInInspector] public GameObject contentPanel; 
    public GameObject buttonPrefab;

    private void Start()
    {
    }

    void FindReferences()
    {
        attackButton = actionButtons?.transform.Find("AttackBTN")?.GetComponent<Button>();
        skillsButton = actionButtons?.transform.Find("SkillsBTN")?.GetComponent<Button>();
        itemsButton = actionButtons?.transform.Find("ItemsBTN")?.GetComponent<Button>();

        Canvas canvas = GameObject.FindObjectOfType<Canvas>();        
        playerHUD = canvas.transform.Find("PlayerHUD")?.gameObject;
        enemyHUD = canvas.transform.Find("EnemyHUD")?.gameObject;
        actionButtons = canvas.transform.Find("HUD_ActionButtons")?.gameObject;
        actionsList = canvas.transform.Find("HUD_ActionsList")?.gameObject;
        contentPanel = canvas.transform.Find("HUD_ActionsList/Scroll View/Viewport/Content")?.gameObject;

        combatManager = FindObjectOfType<CombatManager>();


        if (playerHUD == null || enemyHUD == null)
        {
            Debug.LogError("PlayerHUD or EnemyHUD not found!");
        }
    }

    public void SetupCombatUI(SkillComponent skillComponent, InventoryComponent inventoryComponent)
    {
        FindReferences();
        HideActionsList();
        ShowPlayerHUD();
        ShowEnemyHUD(); //I want to make a skill Analize to enable this on each battle

        // Setup button listeners
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();

            // Call PerformAttack directly from CombatManager
            attackButton.onClick.AddListener(() =>
            {               
                // Check if the current state is PlayerTurnState
                if (combatManager.currentState is PlayerTurnState playerTurnState)
                {
                    //HideActionsList();
                    //HideActionButtons();
                    // Call the PlayerAttack function from PlayerTurnState
                    playerTurnState.PlayerAttack();
                }
                else Debug.Log("It is not PlayerTurnState! currentState is " + combatManager.currentState);


            });
        }

        if (skillsButton != null)
        {
            skillsButton.onClick.RemoveAllListeners();
            skillsButton.onClick.AddListener(() =>
            {
                if (actionsList.activeSelf) HideActionsList();
                else OpenActionsList();

                PopulateSkillButtons(skillComponent);
            });
        }

        if (itemsButton != null)
        {
            itemsButton.onClick.RemoveAllListeners();
            itemsButton.onClick.AddListener(() =>
            {
                if (actionsList.activeSelf) HideActionsList();
                else OpenActionsList();

                PopulateItemButtons(inventoryComponent);
            });
        }
    }

    public void OpenActionsList()
    {
        if (actionsList != null)
        {
            actionsList.SetActive(true);
        }
    }

    public void HideActionsList()
    {
        if (actionsList != null)
        {
            actionsList.SetActive(false);
        }
    }

    public void ShowActionButtons()
    {
        
        if (actionButtons != null)
        {
            actionButtons.SetActive(true);
        }
    }

    public void HideActionButtons()
    {
        if (actionButtons != null)
        {
            actionButtons.SetActive(false);
        }
    }

    // Show and hide PlayerHUD
    public void ShowPlayerHUD()
    {
        if (playerHUD != null)
        {
            playerHUD.SetActive(true);
        }
    }

    public void HidePlayerHUD()
    {
        if (playerHUD != null)
        {
            playerHUD.SetActive(false);
        }
    }

    // Show and hide EnemyHUD
    public void ShowEnemyHUD()
    {
        if (enemyHUD != null)
        {
            enemyHUD.SetActive(true);
        }
    }
    public void HideEnemyHUD()
    {
        if (enemyHUD != null)
        {
            enemyHUD.SetActive(false);
        }
    }

    // Populate the UI with skill buttons
    public void PopulateSkillButtons(SkillComponent skillComponent)
    {
        ClearButtons(); // Clear previously created buttons

        foreach (var skill in skillComponent.availableSkills)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentPanel.transform);
            Button button = newButton.GetComponent<Button>();
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = skill.actionName;

            button.onClick.AddListener(() => 
            {
                print("skill used! mana:" + combatManager.playerSkillComponent.currentMana);
                skillComponent.UseSkill(skill, combatManager.enemyStatus); HideActionsList(); 
            });
        }
    }

    // Populate the UI with item buttons
    public void PopulateItemButtons(InventoryComponent inventoryComponent)
    {
        ClearButtons(); // Clear previously created buttons

        foreach (var item in inventoryComponent.availableItems)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentPanel.transform);
            Button button = newButton.GetComponent<Button>();
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = item.actionName;

            button.onClick.AddListener(() => inventoryComponent.UseItem(item, null));
        }
    }

    // Clear all buttons in the content panel
    private void ClearButtons()
    {
        // Ensure contentPanel is not null
        if (contentPanel != null)
        {
            // Loop through all children of the content panel
            foreach (Transform child in contentPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else print("content panel null");
    }
}