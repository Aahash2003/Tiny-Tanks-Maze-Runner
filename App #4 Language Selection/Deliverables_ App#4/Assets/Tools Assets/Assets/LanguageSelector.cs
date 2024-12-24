using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LanguageSelector : MonoBehaviour
{
    public Dropdown languageDropdown; // Reference to the Dropdown in the Inspector

    private void Start()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("LocalizationManager instance is not available.");
            return;
        }

        if (languageDropdown == null)
        {
            Debug.LogError("Dropdown not assigned in the Inspector");
            return;
        }

        // Populate the dropdown and set the default language
        PopulateDropdown();

        // Listen for language changes via the dropdown
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // Set the dropdown to match the saved language
        int savedLanguageIndex = PlayerPrefs.GetInt("SelectedLanguage", 0);
        languageDropdown.value = savedLanguageIndex;
        languageDropdown.RefreshShownValue();

        // Ensure the selected language is active in the LocalizationManager
        LocalizationManager.Instance.SetLanguage(savedLanguageIndex);
    }

    // Populate the dropdown with the available languages
    private void PopulateDropdown()
    {
        // Get the list of languages from LocalizationManager
        var languages = LocalizationManager.Instance.languages;

        // Validate language data
        if (languages == null || languages.Length == 0)
        {
            Debug.LogError("No languages found in LocalizationManager.");
            return;
        }

        // Clear existing options
        languageDropdown.ClearOptions();

        // Add language names to the dropdown
        List<string> languageNames = new List<string>();
        foreach (var language in languages)
        {
            languageNames.Add(language.languageName);
        }

        languageDropdown.AddOptions(languageNames);
    }

    // Handle language changes when the dropdown value is updated
    private void OnLanguageChanged(int index)
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("LocalizationManager instance is missing.");
            return;
        }

        // Update the selected language in LocalizationManager
        LocalizationManager.Instance.SetLanguage(index);

        // Save the selected language index for persistence
        PlayerPrefs.SetInt("SelectedLanguage", index);
        PlayerPrefs.Save();

        // Update all localized text in the scene
        foreach (var localizedText in FindObjectsOfType<LocalizedText>())
        {
            localizedText.UpdateText();
        }

        Debug.Log($"Language changed to: {LocalizationManager.Instance.languages[index].languageName}");
    }
}
