using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class BattleTextManager : MonoBehaviour
{
    public static BattleTextManager Instance { get; private set; } // Singleton instance

    [HideInInspector]
    public TMP_Text dialogueText;

    private bool isWaitingForInput = false;
    private Action onInputAction; // Action to call when the player presses a button

    void Awake()
    {
        FindTargetTextBox();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the instance across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    public void FindTargetTextBox()
    {
        dialogueText = GameObject.FindWithTag("DialogueText")?.GetComponent<TMP_Text>();
    }

    public void DisplayMessage(string message)
    {
        FindTargetTextBox();
        dialogueText.text = message;
    }

    public void DisplayMessageWithInput(string message, Action callback)
    {
        FindTargetTextBox();
        DisplayMessage(message);
        isWaitingForInput = true;
        onInputAction = callback; // Set the action to call when input is received
    }

    // Method to display a message, wait for an effect to finish (like a particle system), and then call a callback
    public void DisplayMessageAndWaitEffect(string message, IEnumerator effect, Action callback)
    {
        StartCoroutine(DisplayMessageAndWaitForEffect(message, effect, callback));
    }

    private IEnumerator DisplayMessageAndWaitForEffect(string message, IEnumerator effect, Action callback)
    {
        FindTargetTextBox();
        // Show the message
        dialogueText.text = message;

        // Wait for the effect (e.g., a particle system or animation) to finish
        yield return StartCoroutine(effect);

        // Once the effect finishes, execute the next action (callback)
        callback?.Invoke();
    }

    // Method to display a message for a set delay and execute a callback afterward
    public void DisplayMessageForSeconds(string message, float delayInSeconds, Action callback)
    {
        StartCoroutine(DisplayDelayMessageCoroutine(message, delayInSeconds, callback));
    }

    // Coroutine to handle the message display, delay, and execute an action afterward
    private IEnumerator DisplayDelayMessageCoroutine(string message, float delayInSeconds, Action callback)
    {
        FindTargetTextBox();
        // Display the message
        dialogueText.text = message;

        // Wait for the specified delay
        yield return new WaitForSeconds(delayInSeconds);

        // Clear the message
        dialogueText.text = "";

        // Execute the callback action
        callback?.Invoke();
    }


    void Update()
    {
        if (isWaitingForInput && Input.anyKeyDown)
        {
            isWaitingForInput = false;
            onInputAction?.Invoke(); // Call the action if set
        }
    }

    public void ClearMessage()
    {
        FindTargetTextBox();
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }


}