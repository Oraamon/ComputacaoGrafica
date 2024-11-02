using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject storedItem;
    public Transform displayPosition; 

    public bool CanPlaceItem(GameObject item)
    {
        return storedItem == null;
    }

    public void PlaceItem(GameObject item)
    {
        if (CanPlaceItem(item) && displayPosition != null)
        {
            storedItem = item;
            item.transform.SetParent(transform); 
            item.transform.position = displayPosition.position; 
            item.transform.rotation = displayPosition.rotation; 
        }
        else
        {
            Debug.LogWarning("Não foi possível colocar o item. Verifique se o DisplayPosition está configurado.");
        }
    }

    public GameObject RemoveItem()
    {
        if (storedItem != null)
        {
            GameObject item = storedItem;
            storedItem = null;
            item.transform.SetParent(null); 
            return item;
        }
        return null; 
    }
}
