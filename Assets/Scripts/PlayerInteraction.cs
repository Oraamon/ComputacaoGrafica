using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public Transform handTransform;
    private GameObject heldItem = null;
    private GameObject nearbyObject = null;
    public Text interactionText;

    private List<GameObject> interactableObjects = new List<GameObject>();

    void Start()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("interactionText não está atribuído.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyObject != null)
        {
            if (heldItem == null) 
            {
                if (nearbyObject.CompareTag("Pickup"))
                {
                    PickupItem(nearbyObject);
                }
                else if (nearbyObject.TryGetComponent<Container>(out Container container))
                {
                    GameObject itemFromContainer = container.SpawnItem(); // Usando SpawnItem para instanciar o queijo
                    if (itemFromContainer != null)
                    {
                        PickupItem(itemFromContainer);
                    }
                }
                
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(false);
                }
            }
            else 
            {
                if (nearbyObject.TryGetComponent<Container>(out Container container))
                {
                    if (container.CanPlaceItem(heldItem))
                    {
                        container.PlaceItem(heldItem);
                        heldItem = null;
                        if (interactionText != null)
                        {
                            interactionText.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    DropItem(transform.position + transform.forward);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup") || other.CompareTag("Interactable"))
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
            nearbyObject = interactableObjects[interactableObjects.Count - 1];
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
            }
        }
        else
        {
            nearbyObject = null;
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    public void PickupItem(GameObject item)
    {
        if (item != null && handTransform != null && item.CompareTag("Pickup"))
        {
            heldItem = item;
            item.transform.SetParent(handTransform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Tentativa de pegar um item inválido.");
        }
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
