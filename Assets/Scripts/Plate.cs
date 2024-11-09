using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private List<GameObject> storedItems = new List<GameObject>();
    public float itemSpacing = 0.3f;
    public float itemHeightOffset = 0.1f;
    public int maxItems = 5;
    private bool canAddItems = false;

    public GameObject carneFritaPrefab;

    public bool CanPlaceItem(GameObject item)
    {
        return canAddItems && storedItems.Count < maxItems;
    }

    public void PlaceItem(GameObject item)
    {
        if (CanPlaceItem(item))
        {
            storedItems.Add(item);
            item.transform.SetParent(transform);

            item.tag = "StoredOnPlate";

            ArrangeItems();
            CheckForRecipe();
        }
        else
        {
            Debug.LogWarning("Não foi possível colocar o item no prato.");
        }
    }

    private void CheckForRecipe()
    {
        bool hasQueijo = false;
        bool hasCarne = false;

        foreach (GameObject item in storedItems)
        {
            if (item.name.Contains("Queijo"))
            {
                hasQueijo = true;
            }
            else if (item.name.Contains("Carne"))
            {
                hasCarne = true;
            }
        }

        if (hasQueijo && hasCarne)
        {
            foreach (GameObject storedItem in storedItems)
            {
                Destroy(storedItem);
            }
            storedItems.Clear();

            Debug.Log("carne assada feita");

            GameObject newItem = Instantiate(carneFritaPrefab, transform);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;

            storedItems.Add(newItem);

            newItem.tag = "StoredOnPlate";

            ArrangeItems();
        }
    }

    public void RemoveItem(GameObject item)
    {
        if (storedItems.Contains(item))
        {
            storedItems.Remove(item);
            item.transform.SetParent(null);

            item.tag = "Pickup";

            ArrangeItems();
        }
    }

    private void ArrangeItems()
    {

        float angleStep = 360f / storedItems.Count;
        for (int i = 0; i < storedItems.Count; i++)
        {
            float angle = i * angleStep;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * itemSpacing - 0.2f,
                itemHeightOffset,
                Mathf.Sin(angle * Mathf.Deg2Rad) * itemSpacing
            );

            storedItems[i].transform.localPosition = offset;
            storedItems[i].transform.localRotation = Quaternion.identity;
        }
    }

    public void SetCanAddItems(bool value)
    {
        canAddItems = value;
        if (value)
        {
            Debug.Log("Agora o prato pode receber itens.");
        }
    }

    public List<string> GetStoredItemNames()
    {
        List<string> itemNames = new List<string>();
        foreach (GameObject item in storedItems)
        {
            itemNames.Add(item.name);
        }
        return itemNames;
    }

}
