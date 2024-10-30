using UnityEngine;

public class Prato : Item
{
    public bool hasFood = false;

    public void PlaceFood(Item food)
    {
        if (!hasFood && food.currentState == ItemState.Cozido)
        {
            hasFood = true;
            Destroy(food.gameObject); // Remove o item de alimento
        }
    }
}
