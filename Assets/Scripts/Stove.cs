using UnityEngine;

public class Stove : MonoBehaviour
{
    public GameObject storedItem; // Item atualmente no fogão
    public Transform displayPosition; // Posição para o item no fogão
    public GameObject cookedItemPrefab; // Prefab da versão cozida do item
    public ProgressTracker progressTracker; // Referência ao ProgressTracker
    public ProgressBarUI progressBarUI; // Referência ao UI da barra de progresso

    public float cookingTime = 5f; // Tempo necessário para cozinhar o item

    private bool isCooking = false; // Verifica se o fogão está cozinhando

    private void Start()
    {
        if (progressTracker != null)
        {
            progressTracker.SetTotalWork(cookingTime);
            progressTracker.OnProgressChanged += HandleProgressChanged;
            progressTracker.OnProgressComplete += HandleCookingComplete;
        }
        else
        {
            Debug.LogWarning("ProgressTracker is not assigned to the Stove.");
        }
    }

    private void HandleProgressChanged(double progress)
    {
        if (progressBarUI != null)
        {

        }
    }

    private void HandleCookingComplete()
    {
        CookItem();
    }


    private void Update()
    {
        // Atualiza o progresso do cozimento se um item está sendo cozido
        if (isCooking && storedItem != null)
        {
            progressTracker.AddWorkDone(Time.deltaTime);
        }
    }

    public bool CanPlaceItem(GameObject item)
    {
        return storedItem == null;
    }

    public void PlaceItem(GameObject item)
    {
        if (CanPlaceItem(item))
        {
            storedItem = item;
            item.transform.SetParent(transform);
            item.transform.position = displayPosition.position;
            item.transform.rotation = displayPosition.rotation;

            // Inicia o processo de cozimento
            StartCooking();
        }
        else
        {
            Debug.LogWarning("O fogão já está ocupado.");
        }
    }

    private void StartCooking()
    {
        if (progressTracker != null)
        {
            progressTracker.ResetProgress(); // Zera o progresso
            progressTracker.SetTotalWork(cookingTime); // Define o tempo total de cozimento
        }

        if (progressBarUI != null)
        {

        }

        isCooking = true; // Inicia o cozimento
    }


    private void CookItem()
    {
        // Para o processo de cozimento
        isCooking = false;

        if (progressBarUI != null)
        {

        }

        // Remove o item cru
        Destroy(storedItem);

        // Substitui pelo item cozido
        storedItem = Instantiate(cookedItemPrefab, displayPosition.position, displayPosition.rotation);
        storedItem.transform.SetParent(transform);
        storedItem.tag = "Pickup"; // Define a tag correta para o item cozido
    }

    public GameObject RemoveItem()
    {
        if (storedItem != null)
        {
            GameObject item = storedItem;
            storedItem = null;
            item.transform.SetParent(null);

            // Para o cozimento se o item for removido antes de ficar pronto
            isCooking = false;
            if (progressBarUI != null)
            {

            }

            return item;
        }
        return null;
    }
}
