using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    public string recipeName; 
    public List<Ingredient> ingredients;

    [System.Serializable]
    public class Ingredient
    {
        public string ingredientName;
        public Sprite ingredientImage;
    }
}
