using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public Transform content;  // The content area of the scroll view where buttons will be added
    public GameObject buttonPrefab;  // A button prefab to instantiate for each entry

    public void ClearScrollView()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddButton(string buttonText, System.Action onClickAction)
    {
        GameObject newButton = Instantiate(buttonPrefab, content);  // Instantiate the button in the content area
        Button buttonComponent = newButton.GetComponent<Button>();

        if (buttonComponent != null)
        {
            buttonComponent.GetComponentInChildren<Text>().text = buttonText;
            buttonComponent.onClick.AddListener(() => onClickAction.Invoke());
        }
    }
}