using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject storedItem;
    public Transform displayPosition;

    public GameObject itemPrefab;
    public GameObject SpawnItem()
    {
        if (itemPrefab != null)
        {
            GameObject spawnedItem = Instantiate(itemPrefab);
            spawnedItem.tag = "Pickup"; // Certifique-se de que o item tenha a tag correta
            return spawnedItem;
        }
        else
        {
            Debug.LogWarning("Prefab do item não atribuído.");
            return null;
        }
    }

    public bool CanPlaceItem(GameObject item)
    {
        // Permite colocar um item apenas se o container estiver vazio ou se o item for adicionado ao prato.
        return storedItem == null || (storedItem != null && storedItem.GetComponent<Plate>() != null);
    }

    public void PlaceItem(GameObject item)
    {
        if (displayPosition == null)
        {
            Debug.LogWarning("DisplayPosition não está configurado.");
            return;
        }

        // Verifica se o container já possui um item
        if (storedItem != null)
        {
            // Se o item no container é um prato, tenta adicionar o item ao prato
            if (storedItem.TryGetComponent<Plate>(out Plate plate))
            {
                if (plate.CanPlaceItem(item))
                {
                    plate.PlaceItem(item);
                    item.transform.SetParent(plate.transform);  // Adiciona o item como filho do prato
                    return;
                }
                else
                {
                    Debug.LogWarning("O prato já está cheio e não pode receber mais itens.");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("A bancada já possui um item que não é um prato.");
                return;
            }
        }

        // Caso o container esteja vazio, coloca o item diretamente na bancada
        storedItem = item;
        item.transform.SetParent(transform);
        item.transform.position = displayPosition.position;
        item.transform.rotation = displayPosition.rotation;

        // Se o item é um prato, permite que ele receba itens
        if (item.TryGetComponent<Plate>(out Plate placedPlate))
        {
            placedPlate.SetCanAddItems(true);
        }
    }

    public GameObject RemoveItem()
    {
        if (storedItem != null)
        {
            GameObject item = storedItem;

            if (item.TryGetComponent<Plate>(out Plate plate))
            {
                plate.SetCanAddItems(false);
            }

            storedItem = null;
            item.transform.SetParent(null);
            return item;
        }
        return null;
    }
}
