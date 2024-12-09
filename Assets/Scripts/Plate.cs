using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [Header("Configurações de Itens")]
    [SerializeField] private Transform itemDisplayPosition; // Ponto central para posicionamento dos itens
    [SerializeField] private float itemSpacing = 0.3f; // Distância do centro para cada item
    [SerializeField] private float itemHeightOffset = 0.1f; // Deslocamento vertical fixo
    [SerializeField] private int maxItems = 5; // Número máximo de itens no prato
    private bool canAddItems = false; // Flag para permitir ou não adicionar itens

    [Header("Prefabs")]
    [SerializeField] private GameObject carneFritaPrefab; // Prefab para "Carne Frita"

    private List<GameObject> storedItems = new List<GameObject>(); // Lista de itens armazenados no prato

    private void Awake()
    {
        // Verifica se o itemDisplayPosition foi atribuído no Inspector
        if (itemDisplayPosition == null)
        {
            Debug.LogError("ItemDisplayPosition não está atribuído no Inspector.");
        }
    }

    /// <summary>
    /// Verifica se um item pode ser colocado no prato
    /// </summary>
    /// <param name="item">O GameObject a ser colocado</param>
    /// <returns>True se o item pode ser colocado, caso contrário, false</returns>
    public bool CanPlaceItem(GameObject item)
    {
        return canAddItems && storedItems.Count < maxItems;
    }

    /// <summary>
    /// Adiciona um item ao prato
    /// </summary>
    /// <param name="item">O GameObject a ser adicionado</param>
    public void PlaceItem(GameObject item)
    {
        if (CanPlaceItem(item))
        {
            storedItems.Add(item);
            item.transform.SetParent(itemDisplayPosition); // Define o parent como itemDisplayPosition
            item.transform.localRotation = Quaternion.identity; // Reseta a rotação local

            item.tag = "StoredOnPlate";

            ArrangeItems(); // Organiza os itens no prato
            CheckForRecipe(); // Verifica se alguma receita foi completada
        }
        else
        {
            Debug.LogWarning("Não foi possível colocar o item no prato.");
        }
    }

    /// <summary>
    /// Verifica se a combinação de itens no prato corresponde a uma receita
    /// </summary>
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
            // Destroi todos os itens atuais no prato
            foreach (GameObject storedItem in storedItems)
            {
                Destroy(storedItem);
            }
            storedItems.Clear();

            Debug.Log("Carne assada feita");

            // Instancia o prefab da Carne Frita no itemDisplayPosition
            GameObject newItem = Instantiate(carneFritaPrefab, itemDisplayPosition);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;

            storedItems.Add(newItem);
            newItem.tag = "StoredOnPlate";

            ArrangeItems(); // Reorganiza os itens após a criação da nova receita
        }
    }

    /// <summary>
    /// Remove um item específico do prato
    /// </summary>
    /// <param name="item">O GameObject a ser removido</param>
    public void RemoveItem(GameObject item)
    {
        if (storedItems.Contains(item))
        {
            storedItems.Remove(item);
            item.transform.SetParent(transform); // Retorna o parent para o prato
            item.transform.localRotation = Quaternion.identity; // Reseta a rotação local

            item.tag = "Pickup";

            ArrangeItems(); // Reorganiza os itens após a remoção
        }
    }

    /// <summary>
    /// Organiza os itens no prato em torno do itemDisplayPosition
    /// </summary>
    private void ArrangeItems()
    {
        if (storedItems.Count == 0) return;

        Vector3[] offsets = new Vector3[storedItems.Count];

        if (storedItems.Count == 1)
        {
            // Um único item é colocado diretamente no itemDisplayPosition
            offsets[0] = Vector3.zero;
        }
        else
        {
            // Organiza os itens em uma disposição circular ao redor do itemDisplayPosition
            float angleStep = 360f / storedItems.Count;
            for (int i = 0; i < storedItems.Count; i++)
            {
                float angle = i * angleStep;
                float rad = angle * Mathf.Deg2Rad;
                float x = Mathf.Cos(rad) * itemSpacing;
                float z = Mathf.Sin(rad) * itemSpacing;
                float y = 0; // Posição Y fixa

                offsets[i] = new Vector3(x, y, z);
            }
        }

        // Aplica as posições com deslocamento vertical fixo
        for (int i = 0; i < storedItems.Count; i++)
        {
            // Define uma posição Y fixa, sem incremento baseado no índice
            Vector3 finalPosition = offsets[i] + new Vector3(0, itemHeightOffset, 0);
            storedItems[i].transform.localPosition = finalPosition;
            storedItems[i].transform.localRotation = Quaternion.identity;

            Debug.Log($"Item {storedItems[i].name} posicionado em {finalPosition} relativo a {itemDisplayPosition.name}");
        }
    }

    /// <summary>
    /// Define se o prato pode ou não receber itens
    /// </summary>
    /// <param name="value">True para permitir, false para não permitir</param>
    public void SetCanAddItems(bool value)
    {
        canAddItems = value;
        if (value)
        {
            Debug.Log("Agora o prato pode receber itens.");
        }
        else
        {
            Debug.Log("Agora o prato não pode receber mais itens.");
        }
    }

    /// <summary>
    /// Obtém os nomes dos itens armazenados no prato
    /// </summary>
    /// <returns>Lista de nomes dos itens</returns>
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
