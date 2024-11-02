using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private List<GameObject> storedItems = new List<GameObject>();
    public Transform displayPosition;
    public float itemSpacing = 0.3f;
    public float itemHeightOffset = 0.1f;
    public int maxItems = 5;
    private bool canAddItems = false;

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

            // Altera a tag para impedir interação direta
            item.tag = "StoredOnPlate"; // Define uma tag específica para itens no prato

            ArrangeItems();
        }
        else
        {
            Debug.LogWarning("Não foi possível colocar o item no prato.");
        }
    }

    public void RemoveItem(GameObject item)
    {
        if (storedItems.Contains(item))
        {
            storedItems.Remove(item);
            item.transform.SetParent(null);

            // Restaura a tag original para permitir interação
            item.tag = "Pickup"; // Altere para a tag original, como "Pickup" ou a tag que você utiliza

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
                Mathf.Cos(angle * Mathf.Deg2Rad) * itemSpacing,
                itemHeightOffset,
                Mathf.Sin(angle * Mathf.Deg2Rad) * itemSpacing
            );

            storedItems[i].transform.position = displayPosition.position + offset;
            storedItems[i].transform.rotation = displayPosition.rotation;
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
}
