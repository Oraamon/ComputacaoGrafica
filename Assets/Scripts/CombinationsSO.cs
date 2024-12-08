using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CombinationsSO : ScriptableObject
{
    [Header("Informações da Receita")]
    [Tooltip("Nome da receita resultante.")]
    public string recipeName; 

    [Tooltip("Prefab que será instanciado quando a combinação for satisfeita.")]
    public GameObject resultPrefab;

    [Header("Ingredientes Necessários")]
    [Tooltip("Lista de ingredientes necessários para a combinação.")]
    public List<Ingredient> ingredients;

    [System.Serializable]
    public class Ingredient
    {
        [Tooltip("Nome do ingrediente.")]
        public string ingredientName;

        [Tooltip("Imagem representativa do ingrediente.")]
        public Sprite ingredientImage;
    }
}
