using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemState { Cru, Cozido }
    public ItemState currentState = ItemState.Cru;

    public bool isHeld = false;
}
