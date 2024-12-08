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

        // Inicialize expectedRecipes se não estiver inicializada no Inspector
        if (expectedRecipes == null)
        {
            expectedRecipes = new List<RecipeSO>();
        }
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                if (recipeListSO == null || recipeListSO.recipeSOList == null || recipeListSO.recipeSOList.Count == 0)
                {
                    Debug.LogError("recipeListSO ou recipeListSO.recipeSOList não está atribuído corretamente.");
                    return;
                }

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

            RecipeSO deliveredRecipe = CheckDelivery();
            if (deliveredRecipe != null)
            {
                CompleteDelivery(deliveredRecipe);
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

    private RecipeSO CheckDelivery()
    {
        if (currentPlate == null)
        {
            Debug.LogWarning("Nenhum prato no delivery para verificar.");
            return null;
        }

        if (expectedRecipes == null)
        {
            Debug.LogError("expectedRecipes não foi inicializada.");
            return null;
        }

        var storedItemNames = currentPlate.GetStoredItemNames();
        if (storedItemNames == null)
        {
            Debug.LogError("currentPlate.GetStoredItemNames() retornou null.");
            return null;
        }

        var deliveredRecipes = storedItemNames.ConvertAll(RemoveCloneFromName);
        Debug.Log($"Receitas entregues: {string.Join(", ", deliveredRecipes)}");
        Debug.Log($"Receitas esperadas: {string.Join(", ", expectedRecipes.ConvertAll(r => r?.recipeName ?? "null"))}");

        foreach (string deliveredRecipe in deliveredRecipes)
        {
            foreach (var expectedRecipe in expectedRecipes)
            {
                if (expectedRecipe == null)
                {
                    Debug.LogWarning("Encontrado um RecipeSO null na lista expectedRecipes.");
                    continue;
                }

                if (expectedRecipe.recipeName == null)
                {
                    Debug.LogWarning($"RecipeSO com nome null: {expectedRecipe}");
                    continue;
                }

                if (NormalizeString(expectedRecipe.recipeName) == NormalizeString(deliveredRecipe))
                {
                    Debug.Log($"Receita encontrada! Entrega correta: {expectedRecipe.recipeName}");
                    return expectedRecipe; // Retorna a receita entregue
                }
            }
        }

        Debug.LogWarning("Nenhuma das receitas entregues corresponde a uma receita esperada.");
        return null;
    }

    private string NormalizeString(string input)
    {
        return input.ToLower().Trim();
    }

    private string RemoveCloneFromName(string itemName)
    {
        return itemName.Replace("(Clone)", "").Trim();
    }

    private void CompleteDelivery(RecipeSO deliveredRecipe)
    {
        Debug.Log($"Entrega completa! Receita '{deliveredRecipe.recipeName}' entregue com sucesso.");

        if (currentPlate != null)
        {
            Destroy(currentPlate.gameObject);
            currentPlate = null;
        }

        // Remove a receita entregue da lista expectedRecipes
        if (expectedRecipes.Contains(deliveredRecipe))
        {
            expectedRecipes.Remove(deliveredRecipe);
            Debug.Log($"Receita '{deliveredRecipe.recipeName}' removida da lista de receitas esperadas.");
        }
        else
        {
            Debug.LogWarning($"Receita '{deliveredRecipe.recipeName}' não encontrada na lista de receitas esperadas.");
        }

        // Remove a receita entregue da lista waitingRecipeSOList
        if (waitingRecipeSOList.Contains(deliveredRecipe))
        {
            waitingRecipeSOList.Remove(deliveredRecipe);
            Debug.Log($"Receita '{deliveredRecipe.recipeName}' removida da lista de receitas aguardando.");
        }
        else
        {
            Debug.LogWarning($"Receita '{deliveredRecipe.recipeName}' não encontrada na lista de receitas aguardando.");
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }
}
