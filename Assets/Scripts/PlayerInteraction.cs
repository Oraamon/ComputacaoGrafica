using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Data.Common;

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
                    GameObject itemFromContainer = container.RemoveItem();
                    if (itemFromContainer != null)
                    {
                        PickupItem(itemFromContainer);
                    }
                }
                else if (nearbyObject.TryGetComponent<FoodContainer>(out FoodContainer foodContainer))
                {
                    GameObject itemFromContainer = foodContainer.SpawnItem();
                    if (itemFromContainer != null)
                    {
                        PickupItem(itemFromContainer);
                    }
                }
                else if (nearbyObject.TryGetComponent<DeliverySpot>(out DeliverySpot delivery))
                {
                    Plate plateFromDelivery = delivery.RemoveItem();

                    if (plateFromDelivery != null)
                    {
                        PickupItem(plateFromDelivery.gameObject);

                        if (interactionText != null)
                        {
                            interactionText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Não há pratos disponíveis no DeliverySpot para pegar.");
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
                else if (nearbyObject.TryGetComponent<DeliverySpot>(out DeliverySpot delivery))
                {
                    if (heldItem.TryGetComponent<Plate>(out Plate plate) && delivery.CanPlacePlate(plate))
                    {
                        delivery.PlacePlate(plate);
                        heldItem = null;

                        if (interactionText != null)
                        {
                            interactionText.gameObject.SetActive(false); 
                        }
                    }
                    else
                    {
                        Debug.LogWarning("O objeto na mão não é um prato ou o delivery não pode aceitar o prato.");
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
        if (other.CompareTag("Interactable"))
        {
            interactableObjects.Add(other.gameObject);
            UpdateNearestObject();

            // Stop blinking on the previously closest object
            foreach (var obj in interactableObjects)
            {
                BlinkingEffect blinkingEffect = obj.GetComponent<BlinkingEffect>();
                if (blinkingEffect != null)
                {
                    blinkingEffect.StopBlinking();
                }
            }

            // Apply blinking effect only to the current nearest object
            if (nearbyObject != null)
            {

                BlinkingEffect blinkingEffect = nearbyObject.GetComponent<BlinkingEffect>();
                if (blinkingEffect != null)
                {
                    blinkingEffect.StartBlinking();
                }
            }
        }
        else if (other.CompareTag("Pickup"))
        {
            interactableObjects.Add(other.gameObject);

            UpdateNearestObject();
        }
    }



    private void OnTriggerExit(Collider other)
    {
        // Certifique-se de que o objeto ainda existe antes de acessá-lo
        if (other == null || other.gameObject == null)
        {
            return;
        }

        if (interactableObjects.Contains(other.gameObject))
        {
            interactableObjects.Remove(other.gameObject);
            UpdateNearestObject();

            // Para o efeito de piscar
            if (other.TryGetComponent<BlinkingEffect>(out BlinkingEffect blinkingEffect))
            {
                blinkingEffect.StopBlinking();
            }

            // Verifica se o nearbyObject ainda é válido
            if (nearbyObject != null && nearbyObject.TryGetComponent<BlinkingEffect>(out BlinkingEffect blinkingEffectAdx))
            {
                blinkingEffectAdx.StartBlinking();
            }
        }
    }


    private void UpdateNearestObject()
    {
        // Remove objetos destruídos da lista
        interactableObjects.RemoveAll(obj => obj == null);

        if (interactableObjects.Count > 0)
        {
            nearbyObject = interactableObjects[interactableObjects.Count - 1];
            
            // Loga os objetos válidos na lista
            Debug.Log("Objetos interagíveis próximos:");
            foreach (var obj in interactableObjects)
            {
                if (obj != null)
                {
                    Debug.Log(obj.name);
                }
            }
        }
        else
        {
            nearbyObject = null;
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

            // Update nearest object after picking up the item
            UpdateNearestObject();
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

            // Update nearest object after dropping the item
            UpdateNearestObject();
        }
    }

}
