using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationKeys))]
public class LocalizationKeysEditor : Editor
{
    private string newKey = ""; // Temporary field to store new key input

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LocalizationKeys keyRepo = (LocalizationKeys)target;

        // Input field for new key
        newKey = EditorGUILayout.TextField("New Key", newKey);

        if (GUILayout.Button("Add New Key") && !string.IsNullOrEmpty(newKey))
        {
            // Add new key if it doesn't already exist
            if (!keyRepo.keys.Contains(newKey))
            {
                keyRepo.keys.Add(newKey);
                EditorUtility.SetDirty(keyRepo);

                // Trigger key synchronization across all languages
                if (LocalizationManager.Instance != null)
                {
                    LocalizationManager.Instance.SynchronizeKeys();
                }

                // Clear the input field after adding
                newKey = "";
            }
            else
            {
                Debug.LogWarning("Key already exists.");
            }
        }
    }
}
