using UnityEngine;
using System.Collections.Generic;
using Complete;
using System;



public class LocalizationManager : MonoBehaviour
{
    public KeyValuePairs[] languages; // Array of all language assets
    public static LocalizationManager Instance;
    private KeyValuePairs currentLanguage;
    public LocalizationKeys localizationKeys; // Reference to the central LocalizationKeys asset
    private Dictionary<string, string> runtimeKeys = new Dictionary<string, string>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Synchronize keys at startup
        SynchronizeKeys();
        SetLanguage(0); // Set default language
        }

        
    }

    public void SynchronizeKeys()
{
    if (localizationKeys == null || languages == null || languages.Length == 0)
    {
        Debug.LogError("LocalizationKeys or languages not properly set up in LocalizationManager.");
        return;
    }

    // Use the keys from LocalizationKeys as the master list
    List<string> masterKeys = localizationKeys.keys;

    // Synchronize runtime keys
    foreach (var runtimeKey in runtimeKeys.Keys)
    {
        if (!masterKeys.Contains(runtimeKey))
        {
            masterKeys.Add(runtimeKey);
        }
    }

    // Synchronize each language with the master list
    foreach (var language in languages)
    {
        language.SynchronizeKeys(masterKeys);
    }
}


    public void SetLanguage(int index)
{
    if (index < 0 || index >= languages.Length || languages[index] == null)
    {
        Debug.LogError("Invalid language index or null language in LocalizationManager.");
        return;
    }
    currentLanguage = languages[index];
    Debug.Log("Language set to: " + currentLanguage.languageName);
}

public void AddRuntimeKey(string key, string defaultValue = "")
{
    // Check if the key already exists in either runtimeKeys or the current language
    if (runtimeKeys.ContainsKey(key) || localizationKeys.keys.Contains(key))
    {
        Debug.LogWarning($"Key '{key}' already exists. Skipping runtime registration.");
        return;
    }

    // Add the new key with a default value
    runtimeKeys[key] = defaultValue;
    Debug.Log($"Runtime key '{key}' added with default value: {defaultValue}");
}



public string GetLocalizedString(string key, params object[] args)
{
    string value = null;

    // Check runtime keys first
    if (runtimeKeys.TryGetValue(key, out value))
    {
        try
        {
            return string.Format(value, args);
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Formatting error for key '{key}' in runtime keys: {ex.Message}");
            return value;
        }
    }

    // Check predefined keys in the current language
    foreach (var localizedString in currentLanguage.strings)
    {
        if (localizedString.key == key)
        {
            try
            {
                return string.Format(localizedString.value, args);
            }
            catch (FormatException ex)
            {
                Debug.LogError($"Formatting error for key '{key}' in {currentLanguage.languageName}: {ex.Message}");
                return "";
            }
        }
    }

    // Key not found; return a placeholder indicating the missing key
    return $"Key '{key}' not found in {currentLanguage.languageName}";
}



}
