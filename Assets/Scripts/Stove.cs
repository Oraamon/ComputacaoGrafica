using System;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour, IProgressible
{
    [SerializeField] private float cookingTime = 10f; 
    [SerializeField] private List<CookPrefabMapping> cookPrefabMappings; 
    [SerializeField] private Transform displayPosition; 

    private Dictionary<string, GameObject> cookPrefabDictionary; 
    private GameObject currentObject; 
    private float cookingProgress = 0f; 
    private bool isCooking = false; 

    public event EventHandler<ProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnProgressComplete;

    public event EventHandler OnCutPaused; 
    public event EventHandler OnCutCancelled;

    private void Start()
    {
        if (displayPosition == null)
        {
            Debug.LogError("DisplayPosition não está atribuído no Inspector.");
        }

        if (cookPrefabMappings == null || cookPrefabMappings.Count == 0)
        {
            Debug.LogError("CookPrefabMappings não estão configuradas no Inspector.");
        }

        cookPrefabDictionary = new Dictionary<string, GameObject>();
        foreach (CookPrefabMapping mapping in cookPrefabMappings)
        {
            string key = mapping.originalName.ToLower();
            if (!cookPrefabDictionary.ContainsKey(key))
            {
                cookPrefabDictionary.Add(key, mapping.cookedPrefab);
            }
            else
            {
                Debug.LogWarning($"Duplicação encontrada no mapeamento: {mapping.originalName}. Apenas o primeiro mapeamento será usado.");
            }
        }
    }

    private void Update()
    {
        if (isCooking && currentObject != null)
        {
            cookingProgress += Time.deltaTime;

            float progressNormalized = cookingProgress / cookingTime;
            OnProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progressNormalized));

            if (cookingProgress >= cookingTime)
            {
                CompleteCooking();
            }
        }
    }

    private void CompleteCooking()
    {
        isCooking = false;
        OnProgressComplete?.Invoke(this, EventArgs.Empty);

        if (currentObject != null)
        {
            Debug.Log($"Objeto {currentObject.name} cozinhado!");
            Destroy(currentObject);

            GameObject cookedPrefab = GetCookedPrefab(currentObject.name);
            if (cookedPrefab != null)
            {
                GameObject cookedObject = Instantiate(cookedPrefab, displayPosition.position, displayPosition.rotation, transform);
                cookedObject.tag = "Pickup";

                Debug.Log($"Objeto cozinhado instanciado: {cookedObject.name}");
                currentObject = cookedObject;
            }
            else
            {
                Debug.LogError($"Não foi encontrado um prefab cozido para o objeto: {currentObject.name}");
                currentObject = null;
            }

            cookingProgress = 0f;
        }
    }

    public bool TryPlaceObject(GameObject playerObject)
    {
        if (currentObject != null)
        {
            Debug.LogWarning("A stove já está ocupada.");
            return false;
        }

        if (!IsObjectCookable(playerObject))
        {
            Debug.LogWarning($"O objeto {playerObject.name} não é cozinhável.");
            return false;
        }

        if (IsAlreadyCooked(playerObject))
        {
            Debug.LogWarning($"O objeto {playerObject.name} já foi cozinhado e não pode ser cozinhado novamente.");
            return false;
        }

        currentObject = playerObject;
        currentObject.transform.SetParent(transform);
        currentObject.transform.position = displayPosition.position;
        currentObject.SetActive(true);

        Debug.Log($"Objeto {currentObject.name} colocado na stove.");

        cookingProgress = 0f;
        isCooking = true;

        OnProgressChanged?.Invoke(this, new ProgressChangedEventArgs(0f));

        return true;
    }

    public GameObject TakeObject()
    {
        if (currentObject == null) return null;

        GameObject objectToTake = currentObject;
        currentObject = null;
        isCooking = false;
        cookingProgress = 0f;

        objectToTake.transform.parent = null;
        objectToTake.SetActive(true);

        Debug.Log($"Objeto {objectToTake.name} retirado da stove.");
        return objectToTake;
    }

    public void StartCooking()
    {
        if (currentObject == null)
        {
            Debug.LogWarning("Não há nenhum objeto na stove para cozinhar.");
            return;
        }

        if (IsAlreadyCooked(currentObject))
        {
            Debug.LogWarning($"O objeto {currentObject.name} já foi cozinhado e não pode ser cozinhado novamente.");
            return;
        }

        if (isCooking)
        {
            Debug.LogWarning("O cozimento já está em andamento.");
            return;
        }

        isCooking = true;
        Debug.Log("Iniciando o cozimento do objeto na stove.");
    }

    public void PauseCooking()
    {
        if (isCooking)
        {
            isCooking = false;
            Debug.Log("Cozimento pausado.");
            OnCutPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public void CancelCooking()
    {
        if (isCooking)
        {
            isCooking = false;
            cookingProgress = 0f;
            Debug.Log("Cozimento cancelado.");
            OnCutCancelled?.Invoke(this, EventArgs.Empty);

            OnProgressChanged?.Invoke(this, new ProgressChangedEventArgs(cookingProgress / cookingTime));
        }
    }

    public float GetProgressNormalized()
    {
        return cookingProgress / cookingTime;
    }

    private bool IsObjectCookable(GameObject obj)
    {
        string objNameLower = obj.name.ToLower();
        return cookPrefabDictionary.ContainsKey(objNameLower);
    }

    private bool IsAlreadyCooked(GameObject obj)
    {
        return obj.name.ToLower().Contains("_cooked");
    }

    private GameObject GetCookedPrefab(string originalName)
    {
        string key = originalName.ToLower();
        if (cookPrefabDictionary.TryGetValue(key, out GameObject cookedPrefab))
        {
            return cookedPrefab;
        }
        return null;
    }
}
