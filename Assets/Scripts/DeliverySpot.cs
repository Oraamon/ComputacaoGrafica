using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour
{
    [SerializeField] private List<string> expectedRecipes;
    private Plate currentPlate;

    public Transform platePosition;

    public bool CanPlacePlate(Plate plate)
    {
        return currentPlate == null && plate != null;
    }

    public void PlacePlate(Plate plate)
    {
        if (CanPlacePlate(plate))
        {
            currentPlate = plate;
            plate.transform.SetParent(transform);
            plate.transform.position = platePosition.position;
            plate.transform.rotation = platePosition.rotation;

            if (CheckDelivery())
            {
                CompleteDelivery();
            }
            else
            {
                Debug.LogWarning("Receita no prato não corresponde ao esperado.");
            }
        }
        else
        {
            Debug.LogWarning("Já existe um prato no local de entrega.");
        }
    }

    public Plate RemoveItem()
    {
        if (currentPlate != null)
        {
            Plate plateToRemove = currentPlate;
            currentPlate = null;

            // Remove o prato do delivery e redefine sua posição
            plateToRemove.transform.SetParent(null);

            Debug.Log("Prato removido do delivery.");
            return plateToRemove;
        }

        Debug.LogWarning("Nenhum prato no delivery para remover.");
        return null;
    }

    private bool CheckDelivery()
    {
        if (currentPlate == null)
        {
            Debug.LogWarning("Nenhum prato no delivery para verificar.");
            return false;
        }

        // Obtém os nomes normalizados dos itens no prato
        var deliveredRecipes = currentPlate.GetStoredItemNames().ConvertAll(RemoveCloneFromName);
        Debug.LogWarning($"deliveredRecipes: {string.Join(", ", deliveredRecipes)}");

        foreach (string recipe in expectedRecipes)
        {
            if (!deliveredRecipes.Contains(recipe))
            {
                return false;
            }
        }

        return deliveredRecipes.Count == expectedRecipes.Count;
    }

    private string RemoveCloneFromName(string itemName)
    {
        return itemName.Replace("(Clone)", "").Trim();
    }


    private void CompleteDelivery()
    {
        Debug.Log("Entrega completa! Receita entregue com sucesso.");

        if (currentPlate != null)
        {
            Destroy(currentPlate.gameObject);
            currentPlate = null;
        }
    }
}
