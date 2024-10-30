using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject storedItem;

    public bool CanPlaceItem(GameObject item)
    {
        // Verifica se pode armazenar o item (exemplo para simplificação)
        return storedItem == null;
    }

    public void PlaceItem(GameObject item)
    {
        if (CanPlaceItem(item))
        {
            storedItem = item;
            item.transform.SetParent(transform); // Coloca o item na bancada
            item.transform.localPosition = new Vector3(0, 1, 0); // Ajuste a posição de exibição
        }
    }

    public GameObject RemoveItem()
    {
        GameObject item = storedItem;
        storedItem = null;
        item.transform.SetParent(null);
        return item;
    }
}
