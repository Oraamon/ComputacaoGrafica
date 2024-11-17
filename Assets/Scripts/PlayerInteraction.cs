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
        if (interactableObjects.Contains(other.gameObject))
        {
            interactableObjects.Remove(other.gameObject);
            UpdateNearestObject();

            // Loga a lista no console
            Debug.Log("Objetos interagíveis próximos (OnTriggerExit):");
            foreach (var obj in interactableObjects)
            {
                Debug.Log(obj.name);
            }

            // Pare o efeito de piscar
            BlinkingEffect blinkingEffect = other.GetComponent<BlinkingEffect>();
            if (blinkingEffect != null)
            {
                blinkingEffect.StopBlinking();
            }

            if (nearbyObject != null)
            {
                Debug.Log(nearbyObject + "aaaaaaa");
                BlinkingEffect blinkingEffectAdx = nearbyObject.GetComponent<BlinkingEffect>();
                if (blinkingEffectAdx != null)
                {
                    blinkingEffectAdx.StartBlinking();
                }
            }
        }
    }

    private void UpdateNearestObject()
    {
        if (interactableObjects.Count > 0)
        {
            nearbyObject = interactableObjects[interactableObjects.Count - 1];
            // Log the list of interactable objects in a readable format
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
