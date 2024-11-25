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

    // Referência para MesaCorte atual
    private MesaCorte currentMesaCorte = null;

    // UI para mostrar o progresso do corte
    public Slider cuttingProgressSlider;

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

        if (cuttingProgressSlider != null)
        {
            cuttingProgressSlider.gameObject.SetActive(false);
            cuttingProgressSlider.minValue = 0f;
            cuttingProgressSlider.maxValue = 1f;
            cuttingProgressSlider.value = 0f;
        }
        else
        {
            Debug.LogWarning("cuttingProgressSlider não está atribuído.");
        }
    }

    void Update()
    {
        HandleInteractionKey();
        HandleContinuousInteraction();
        HandleCancelCutting(); // Opcional: Implementar cancelamento via tecla
    }

    private void HandleInteractionKey()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyObject != null)
        {
            if (heldItem == null)
            {
                // Não está segurando nenhum item, tentar pegar ou interagir
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
                else if (nearbyObject.TryGetComponent<MesaCorte>(out MesaCorte mesaCorte))
                {
                    // Retirar item da MesaCorte
                    GameObject itemFromMesa = mesaCorte.TakeObject();
                    if (itemFromMesa != null)
                    {
                        PickupItem(itemFromMesa);
                        Debug.Log("Item retirado da mesa de corte.");
                    }
                    else
                    {
                        Debug.LogWarning("Não há itens na mesa de corte para retirar.");
                    }
                }
                else if(nearbyObject.TryGetComponent<Stove>(out Stove stove)){
                    GameObject itemFromProgressible = null;
                    itemFromProgressible = stove.TakeObject();
                    if (itemFromProgressible != null)
                    {
                        PickupItem(itemFromProgressible);
                         Debug.Log("Item retirado do fogão.");
                    }
                    else
                    {
                            Debug.LogWarning("Não há itens no fogão para retirar.");
                    }
                }
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(false);
                }
            }
            else
            {
                // Está segurando um item, tentar colocar
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
                else if (nearbyObject.TryGetComponent<MesaCorte>(out MesaCorte mesaCorte))
                {
                    if (mesaCorte.TryPlaceObject(heldItem))
                    {
                        heldItem = null;
                        currentMesaCorte = mesaCorte;
                        SubscribeToMesaCorteEvents();
                        Debug.Log("Item colocado na mesa de corte.");
                    }
                    else
                    {
                        Debug.LogWarning("Não é possível colocar o item na mesa de corte.");
                    }
                }
                else if(nearbyObject.TryGetComponent<Stove>(out Stove stove)){
                    bool placed = false;
                    placed = stove.TryPlaceObject(heldItem);
                    if (placed)
                    {

                    }
                    else
                    {
                        Debug.LogWarning("Não é possível colocar o item no fogão.");
                    }
                }
                else
                {
                    DropItem(transform.position + transform.forward);
                }
            }
        }
    }

    private void HandleContinuousInteraction()
    {
        // Detectar se a tecla 'F' está sendo pressionada
        bool isFPressed = Input.GetKey(KeyCode.F);

        if (isFPressed && nearbyObject != null && nearbyObject.TryGetComponent<MesaCorte>(out MesaCorte mesa))
        {
            if (currentMesaCorte != null && currentMesaCorte == mesa)
            {
                // Já está interagindo com esta mesa de corte
                // Corte já está sendo tratado no MesaCorte.cs
            }
            else
            {
                // Iniciar interação contínua
                mesa.StartCutting();
                currentMesaCorte = mesa;
                SubscribeToMesaCorteEvents();
                Debug.Log("Iniciando corte na mesa de corte.");
            }
        }
        else
        {
            // Se a tecla 'F' não está sendo pressionada, pausar o corte se estiver ativo
            if (currentMesaCorte != null)
            {
                currentMesaCorte.PauseCutting();
                UnsubscribeFromMesaCorteEvents();
                currentMesaCorte = null;
                Debug.Log("Corte pausado.");
            }
        }
    }

    private void HandleCancelCutting()
    {
        // Opcional: Implementar cancelamento do corte via tecla (exemplo: tecla 'G')
        if (Input.GetKeyDown(KeyCode.G) && currentMesaCorte != null)
        {
            currentMesaCorte.CancelCutting();
            UnsubscribeFromMesaCorteEvents();
            currentMesaCorte = null;
            Debug.Log("Corte cancelado pelo jogador.");
        }
    }

    private void SubscribeToMesaCorteEvents()
    {
        if (currentMesaCorte != null)
        {
            // Atualização: Substituir OnCutComplete por OnProgressComplete
            currentMesaCorte.OnProgressChanged += HandleCuttingProgressChanged;
            currentMesaCorte.OnProgressComplete += HandleCutComplete; // Alterado para OnProgressComplete
            currentMesaCorte.OnCutPaused += HandleCutPaused;
            currentMesaCorte.OnCutCancelled += HandleCutCancelled;

            if (cuttingProgressSlider != null)
            {
                cuttingProgressSlider.gameObject.SetActive(true);
                cuttingProgressSlider.value = currentMesaCorte.GetProgressNormalized();
            }
        }
    }

    private void UnsubscribeFromMesaCorteEvents()
    {
        if (currentMesaCorte != null)
        {
            currentMesaCorte.OnProgressChanged -= HandleCuttingProgressChanged;
            currentMesaCorte.OnProgressComplete -= HandleCutComplete; // Alterado para OnProgressComplete
            currentMesaCorte.OnCutPaused -= HandleCutPaused;
            currentMesaCorte.OnCutCancelled -= HandleCutCancelled;
            currentMesaCorte = null;

            if (cuttingProgressSlider != null)
            {
                cuttingProgressSlider.gameObject.SetActive(false);
            }
        }
    }

    // Atualização: Alterar o tipo de argumento para ProgressChangedEventArgs
    private void HandleCuttingProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (cuttingProgressSlider != null)
        {
            cuttingProgressSlider.value = e.ProgressNormalized;
        }
    }

    private void HandleCutComplete(object sender, System.EventArgs e)
    {
        Debug.Log("Corte concluído!");
        UnsubscribeFromMesaCorteEvents();
    }

    private void HandleCutPaused(object sender, System.EventArgs e)
    {
        Debug.Log("Corte pausado.");
        // Opcional: Atualizar a UI para refletir o estado pausado
    }

    private void HandleCutCancelled(object sender, System.EventArgs e)
    {
        Debug.Log("Corte cancelado.");
        // Opcional: Atualizar a UI para refletir o estado cancelado
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable") || other.CompareTag("Pickup"))
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            return;
        }

        if (interactableObjects.Contains(other.gameObject))
        {
            interactableObjects.Remove(other.gameObject);
            UpdateNearestObject();

            if (other.TryGetComponent<BlinkingEffect>(out BlinkingEffect blinkingEffect))
            {
                blinkingEffect.StopBlinking();
            }

            if (nearbyObject != null && nearbyObject.TryGetComponent<BlinkingEffect>(out BlinkingEffect blinkingEffectAdx))
            {
                blinkingEffectAdx.StartBlinking();
            }
        }
    }

    private void UpdateNearestObject()
    {
        interactableObjects.RemoveAll(obj => obj == null);

        if (interactableObjects.Count > 0)
        {
            // Selecionar o objeto mais próximo
            nearbyObject = interactableObjects[interactableObjects.Count - 1];

            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
                interactionText.text = $"Press E to interact with {nearbyObject.name}";
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

            UpdateNearestObject();
        }
        else
        {
            Debug.LogWarning("Tentativa de pegar um item inválido ou handTransform não está atribuído.");
        }
    }

    public void DropItem(Vector3 position)
    {
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem.transform.position = position;
            heldItem.SetActive(true);
            heldItem = null;

            UpdateNearestObject();
        }
    }
}
