using UnityEngine;

public class FoodContainer : MonoBehaviour
{

    public GameObject itemPrefab;
    public GameObject SpawnItem()
    {
        if (itemPrefab != null)
        {
            GameObject spawnedItem = Instantiate(itemPrefab);
            spawnedItem.tag = "Pickup";
            return spawnedItem;
        }
        else
        {
            Debug.LogWarning("Prefab do item não atribuído.");
            return null;
        }
    }
}
