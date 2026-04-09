using UnityEngine;

public class RandomizeBooks : MonoBehaviour
{
    [Header("Drop your colored materials here!")]
    public Material[] bookMaterials;

    void Start()
    {
        MeshRenderer[] allRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer item in allRenderers)
        {
            string itemName = item.gameObject.name;

            if (itemName.StartsWith("Book") && !itemName.Contains("BookCase"))
            {
                int randomIndex = Random.Range(0, bookMaterials.Length);
                item.sharedMaterial = bookMaterials[randomIndex];
            }
        }
    }
}