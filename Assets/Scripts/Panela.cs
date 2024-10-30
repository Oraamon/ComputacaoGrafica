using UnityEngine;

public class Panela : Item
{
    public bool hasIngredient = false;

    public void AddIngredient()
    {
        if (!hasIngredient)
        {
            hasIngredient = true;
            currentState = ItemState.Cru;
        }
    }
}
