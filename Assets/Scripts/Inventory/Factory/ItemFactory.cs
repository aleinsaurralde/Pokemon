using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemFactory")]
public class ItemFactory : ScriptableObject
{
    [Header("Scriptable Objects References")]
    [SerializeField] private List<RecoveryItem> healthItems;

    // Diccionarios para acceso rápido por nombre
    private Dictionary<string, RecoveryItem> healthItemsDict;

    public void Initialize()
    {
        healthItemsDict = new Dictionary<string, RecoveryItem>();

        foreach (var item in healthItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.Name))
                healthItemsDict[item.Name] = item;
        }
    }

    // Métodos para obtener items específicos
    public RecoveryItem GetHealthItem(string itemName)
    {
        if (healthItemsDict != null && healthItemsDict.ContainsKey(itemName))
            return healthItemsDict[itemName];

        Debug.LogWarning($"Health item '{itemName}' not found in factory!");
        return null;
    }
}