using UnityEngine;

public class Delivery : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public string[] requiredIngredients;
    }

    public Recipe[] recipes;  // Array de receitas possíveis

    private Recipe currentRecipe;  // Receita atual selecionada

    private void Start()
    {
        ChooseRandomRecipe();
    }

    private void ChooseRandomRecipe()
    {
        int randomIndex = Random.Range(0, recipes.Length);
        currentRecipe = recipes[randomIndex];

        // Imprime a receita escolhida e os ingredientes necessários no console
        Debug.Log($"Receita escolhida: {currentRecipe.recipeName}");
        Debug.Log("Ingredientes necessários:");
        foreach (string ingredient in currentRecipe.requiredIngredients)
        {
            Debug.Log(ingredient);
        }
    }

    public bool CheckPlate(Plate plate)
    {
        // Verifica se o prato tem o item correto
        var itemsOnPlate = plate.GetStoredItemNames();
        foreach (var ingredient in currentRecipe.requiredIngredients)
        {
            if (!itemsOnPlate.Contains(ingredient))
            {
                Debug.Log("O prato não contém os ingredientes necessários.");
                return false;
            }
        }
        
        Debug.Log("Prato completo! Entrega feita.");
        return true;
    }
}
