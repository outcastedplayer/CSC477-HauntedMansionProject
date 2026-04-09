#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CreateItemAssets
{
    [MenuItem("Tools/Create Inventory Item Assets")]
    public static void CreateAll()
    {
        string folder = "Assets/Items";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "Items");

        CreateItem(folder, "OfficeKey", "office_key", "Office Key", "A brass key found in the office.", true, true);
        CreateItem(folder, "LibraryKey", "library_key", "Library Key", "An old iron key from the library.", true, true);
        CreateItem(folder, "ParlorKey", "parlor_key", "Parlor Key", "A small key hidden in the puzzle box.", true, true);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Created 3 InventoryItem assets in " + folder);
    }

    static void CreateItem(string folder, string fileName, string id, string displayName, string desc, bool isKey, bool consumable)
    {
        string path = folder + "/" + fileName + ".asset";
        if (AssetDatabase.LoadAssetAtPath<InventoryItem>(path) != null)
        {
            Debug.Log("Already exists: " + path);
            return;
        }

        InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
        item.itemID = id;
        item.itemName = displayName;
        item.description = desc;
        item.isKeyItem = isKey;
        item.isConsumable = consumable;

        AssetDatabase.CreateAsset(item, path);
        Debug.Log("Created: " + path);
    }
}
#endif
