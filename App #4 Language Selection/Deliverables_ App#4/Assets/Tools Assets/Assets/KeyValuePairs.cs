using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key-Value Pair", menuName = "Localization/KeyValuePairs")]
public class KeyValuePairs : ScriptableObject
{
    public string languageName;
    public List<LocalizedString> strings = new List<LocalizedString>();

    // Ensures that every KeyValuePairs instance has the same keys
    public void SynchronizeKeys(List<string> masterKeys)
    {
        foreach (var key in masterKeys)
        {
            if (!strings.Exists(ls => ls.key == key))
            {
                strings.Add(new LocalizedString { key = key, value = "" });
            }
        }
    }
}

[System.Serializable]
public class LocalizedString
{
    public string key;
    public string value;
}

