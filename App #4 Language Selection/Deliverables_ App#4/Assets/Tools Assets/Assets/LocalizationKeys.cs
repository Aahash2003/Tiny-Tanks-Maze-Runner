using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationKeys1", menuName = "Localization/LocalizationKeys")]
public class LocalizationKeys : ScriptableObject
{
    public List<string> keys = new List<string>();
}
