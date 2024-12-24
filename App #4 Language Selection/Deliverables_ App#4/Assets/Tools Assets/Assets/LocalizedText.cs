using UnityEngine;
using UnityEngine.UI;
using Complete;
using System;

public class LocalizedText : MonoBehaviour
{
    public string key; // The key for the localized string
    private Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<Text>();

        if (textComponent == null)
        {
            Debug.LogError("Text component not found on the GameObject.");
            return;
        }

        UpdateText();
    }

    public void UpdateText()
{
    if (LocalizationManager.Instance == null)
    {
        Debug.LogWarning("LocalizationManager.Instance is null. Using default text.");
        if (key == "Player")
        {
            textComponent.text = "";
        }
        else
        {
            textComponent.text = key; // Fallback to displaying the key
        }
        return;
    }

    string localizedString = null;

    try
    {
        // Attempt to get the localized string
        localizedString = LocalizationManager.Instance.GetLocalizedString(key);
    }
    catch (FormatException ex)
    {
        // Log and handle the formatting error
        Debug.LogError($"Formatting error for key '{key}': {ex.Message}");
        textComponent.text = ""; // Fallback message
        return;
    }

    if (!string.IsNullOrEmpty(localizedString))
    {
        textComponent.text = localizedString;
    }
    else
    {
        Debug.LogWarning($"Localization key '{key}' not found. Using default text.");
        textComponent.text = ""; // Fallback to displaying the key
    }
}

}
