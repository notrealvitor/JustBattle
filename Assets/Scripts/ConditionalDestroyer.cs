using UnityEngine;
using System.Reflection;

public class ConditionalDestroyer : MonoBehaviour
{
    public bool destroyIfTrue = false;   // Default: destroy if false
    public string className;             // Name of the class (e.g., "SaveSystem")
    public string methodName;            // Name of the method to call (e.g., "SaveFileExists")

    void Start()
    {
        CheckAndDestroy(); // Automatically check and destroy at the start
    }

    // Function to check the condition and destroy the GameObject if the condition matches
    public void CheckAndDestroy()
    {
        if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName))
        {
            // Find the class type by name
            System.Type type = System.Type.GetType(className);

            if (type != null)
            {
                // Get the method by name
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

                if (method != null)
                {
                    // Invoke the method and cast the result to bool
                    bool condition = (bool)method.Invoke(null, null);

                    if (condition == destroyIfTrue)
                    {
                        Debug.Log("GameObject " + gameObject.name + " destroyed based on condition.");
                        Destroy(gameObject);
                        
                    }
                    else
                    {
                        Debug.Log("Condition not met, GameObject " + gameObject.name + " not destroyed.");
                    }
                }
                else
                {
                    Debug.LogError($"Method '{methodName}' not found in class '{className}'");
                }
            }
            else
            {
                Debug.LogError($"Class '{className}' not found.");
            }
        }
        else
        {
            Debug.LogError("Class name or method name is empty!");
        }
    }
}