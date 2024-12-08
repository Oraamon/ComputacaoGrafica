using System.Collections.Generic;
using UnityEngine;

public class DeliverySpot : MonoBehaviour
{
    [SerializeField] private List<RecipeSO> expectedRecipes;
    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private int waitingRecipesMax = 5; 

    private Plate currentPlate;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;

    public Transform platePosition;

    private void Awake()
    {
        waitingRecipeSOList = new List<RecipeSO>();
        spawnRecipeTimer = spawnRecipeTimerMax;
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO newRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log($"Nova receita adicionada: {newRecipeSO.recipeName}");
                waitingRecipeSOList.Add(newRecipeSO);
                expectedRecipes.Add(newRecipeSO);
            }
        }
    }

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

        var deliveredRecipes = currentPlate.GetStoredItemNames().ConvertAll(RemoveCloneFromName);
        Debug.Log($"Receitas entregues: {string.Join(", ", deliveredRecipes)}");
        Debug.Log($"Receitas esperadas: {string.Join(", ", expectedRecipes)}");

        foreach (string deliveredRecipe in deliveredRecipes)
        {
            if (expectedRecipes.Exists(r => NormalizeString(r.recipeName) == NormalizeString(deliveredRecipe)))
            {
                Debug.Log("Receita encontrada! Entrega correta.");
                return true;
            }
        }

        Debug.LogWarning("Nenhuma das receitas entregues corresponde a uma receita esperada.");
        return false;
    }

    private string NormalizeString(string input)
    {
        return input.ToLower().Trim();
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

        if (expectedRecipes.Count > 0)
        {
            expectedRecipes.RemoveAt(0);
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }
}
