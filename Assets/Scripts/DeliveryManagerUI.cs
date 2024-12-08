using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private DeliverySpot deliverySpot; // Referência ao DeliverySpot
    [SerializeField] private Transform container; // Contêiner no Canvas onde as receitas serão exibidas
    [SerializeField] private GameObject recipeTemplate; // Template de uma receita

    private void Start()
    {
        // Desativa o template para que ele não apareça na UI
        if (recipeTemplate != null)
        {
            recipeTemplate.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateRecipeList();
    }

    private void UpdateRecipeList()
    {
        // Remove todas as crianças existentes no contêiner, exceto o template
        foreach (Transform child in container)
        {
            if (child.gameObject != recipeTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        // Obtenha a lista de receitas que ainda faltam
        List<RecipeSO> waitingRecipes = deliverySpot.GetWaitingRecipeSOList();

        // Crie um elemento UI para cada receita com base no template
        foreach (RecipeSO recipe in waitingRecipes)
        {
            GameObject recipeUI = Instantiate(recipeTemplate, container);
            recipeUI.SetActive(true); // Ativa o template instanciado

            // Configura o nome da receita
            Transform recipeNameTransform = recipeUI.transform.Find("RecipeName");
            if (recipeNameTransform != null)
            {
                TextMeshProUGUI recipeNameText = recipeNameTransform.GetComponent<TextMeshProUGUI>();
                if (recipeNameText != null)
                {
                    recipeNameText.text = recipe.recipeName;
                }
                else
                {
                    Debug.LogWarning($"O componente TextMeshProUGUI não foi encontrado no objeto RecipeName.");
                }
            }
            else
            {
                Debug.LogWarning("RecipeName não encontrado no RecipeTemplate.");
            }

            // Configura os ícones de ingredientes
            Transform iconContainerTransform = recipeUI.transform.Find("IconContainer");
            if (iconContainerTransform != null)
            {
                // Remove quaisquer ícones existentes (caso o template já tenha ícones)
                foreach (Transform child in iconContainerTransform)
                {
                    Destroy(child.gameObject);
                }

                // Adiciona um ícone diretamente para cada ingrediente, se existirem
                if (recipe.ingredients != null && recipe.ingredients.Count > 0)
                {
                    foreach (RecipeSO.Ingredient ingredient in recipe.ingredients)
                    {
                        // Cria um novo GameObject para o ícone
                        GameObject ingredientIcon = new GameObject(ingredient.ingredientName);
                        ingredientIcon.transform.SetParent(iconContainerTransform, false);

                        // Adiciona o componente Image e configura o Sprite do ingrediente
                        Image ingredientImage = ingredientIcon.AddComponent<Image>();
                        if (ingredient.ingredientImage != null)
                        {
                            ingredientImage.sprite = ingredient.ingredientImage;
                        }
                        else
                        {
                            Debug.LogWarning($"Imagem não encontrada para o ingrediente {ingredient.ingredientName}.");
                        }

                        // Opcional: Ajusta o tamanho ou outras propriedades do ícone
                        RectTransform rectTransform = ingredientIcon.GetComponent<RectTransform>();
                        rectTransform.sizeDelta = new Vector2(50, 50); // Define um tamanho padrão para os ícones
                    }
                }
                else
                {
                    Debug.Log($"A receita {recipe.recipeName} não possui ingredientes.");
                }
            }
            else
            {
                Debug.LogWarning("IconContainer não encontrado no RecipeTemplate.");
            }

            // Configura o fundo ou outras propriedades, se necessário
            Transform backgroundTransform = recipeUI.transform.Find("Background");
            if (backgroundTransform != null)
            {
            }
            else
            {
                Debug.LogWarning("Background não encontrado no RecipeTemplate.");
            }
        }
    }
}
