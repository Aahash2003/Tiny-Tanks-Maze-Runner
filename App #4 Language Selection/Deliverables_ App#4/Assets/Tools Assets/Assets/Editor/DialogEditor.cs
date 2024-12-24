using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KeyValuePairs))]
public class DialogEditor : Editor
{
    private KeyValuePairs keyValuePairs;

    private void OnEnable()
    {
        keyValuePairs = (KeyValuePairs)target;
    }

    public override void OnInspectorGUI()
    {
        // Title
        EditorGUILayout.LabelField("Dialog Editor", EditorStyles.boldLabel);
        keyValuePairs.languageName = EditorGUILayout.TextField("Language Name", keyValuePairs.languageName);

        // Add button for new key-value pairs
        if (GUILayout.Button("Add New Key-Value Pair"))
        {
            keyValuePairs.strings.Add(new LocalizedString());
        }

        // Iterate over the list of key-value pairs
        for (int i = 0; i < keyValuePairs.strings.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            keyValuePairs.strings[i].key = EditorGUILayout.TextField("Key", keyValuePairs.strings[i].key);
            keyValuePairs.strings[i].value = EditorGUILayout.TextField("Value", keyValuePairs.strings[i].value);

            // Add a delete button for each key-value pair
            if (GUILayout.Button("Delete"))
            {
                keyValuePairs.strings.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // Save changes made in the editor
        if (GUI.changed)
        {
            EditorUtility.SetDirty(keyValuePairs);
        }
    }
}
