using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem = null;
    private GameObject nearbyObject = null;
    public Text interactionText; // Arraste o Text da UI aqui no Inspector

    // Lista para armazenar todos os objetos interativos dentro dos triggers
    private List<GameObject> interactableObjects = new List<GameObject>();

    void Start()
    {
        interactionText.gameObject.SetActive(false); // Esconde o texto no início
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyObject != null)
        {
            if (heldItem == null) // Pegar o objeto
            {
                PickupItem(nearbyObject);
                interactionText.gameObject.SetActive(false); // Esconde o texto ao pegar o item
            }
            else // Soltar o objeto
            {
                if (nearbyObject.TryGetComponent<Container>(out Container container))
                {
                    if (container.CanPlaceItem(heldItem))
                    {
                        container.PlaceItem(heldItem);
                        heldItem = null;
                        interactionText.gameObject.SetActive(false); // Esconde o texto ao soltar o item
                    }
                }
                else
                {
                    DropItem(transform.position + transform.forward); // Solta o item na frente do jogador
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactableObjects.Add(other.gameObject);
            UpdateNearestObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactableObjects.Contains(other.gameObject))
        {
            interactableObjects.Remove(other.gameObject);
            UpdateNearestObject();
        }
    }

    private void UpdateNearestObject()
    {
        if (interactableObjects.Count > 0)
        {
            // Define o objeto mais próximo como o último da lista
            nearbyObject = interactableObjects[interactableObjects.Count - 1];
            interactionText.gameObject.SetActive(true);
        }
        else
        {
            nearbyObject = null;
            interactionText.gameObject.SetActive(false);
        }
    }

    public void PickupItem(GameObject item)
    {
        heldItem = item;
        item.transform.SetParent(transform);
        item.transform.localPosition = new Vector3(0, 1, 0); // Posição para segurar o item
    }

    public void DropItem(Vector3 position)
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem.transform.position = position;
            heldItem = null;
        }
    }
}
