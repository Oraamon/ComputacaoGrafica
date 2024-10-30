using UnityEngine;
using System.Collections;

public class Fogao : Container
{
    public void StartCooking(Panela panela)
    {
        if (panela.hasIngredient && panela.currentState == Item.ItemState.Cru)
        {
            StartCoroutine(CookFood(panela));
        }
    }

    private IEnumerator CookFood(Panela panela)
    {
        Debug.Log("Cozinhando...");
        yield return new WaitForSeconds(3f); // Temporizador de 3 segundos
        panela.currentState = Item.ItemState.Cozido;
        Debug.Log("Alimento pronto!");
    }
}
