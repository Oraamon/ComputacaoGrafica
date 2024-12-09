using System;
using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour, IProgressible
{
    [SerializeField] private float cookingTime = 10f;
    [SerializeField] private List<CookPrefabMapping> cookPrefabMappings;
    [SerializeField] private Transform displayPosition;

    private Dictionary<string, (GameObject cookedPrefab, GameObject finalPrefab)> cookPrefabDictionary;

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

        ValidateMappings();

        cookPrefabDictionary = new Dictionary<string, (GameObject, GameObject)>();
        foreach (CookPrefabMapping mapping in cookPrefabMappings)
        {
            if (mapping.cookedPrefab == null || mapping.finalPrefab == null)
            {
                Debug.LogError($"O mapeamento para {mapping.originalName} está incompleto.");
                continue;
            }

            string key = NormalizeKey(mapping.originalName);
            if (!cookPrefabDictionary.ContainsKey(key))
            {
                cookPrefabDictionary.Add(key, (mapping.cookedPrefab, mapping.finalPrefab));
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

        // Obter o nome do objeto original (normalizado)
        string key = NormalizeKey(currentObject.name);

        // Tentar obter o prefab final do dicionário
        if (!cookPrefabDictionary.TryGetValue(key, out var prefabs))
        {
            Debug.LogError($"Nenhum mapeamento encontrado para o objeto: {currentObject.name}");
            return;
        }

        GameObject finalPrefab = prefabs.finalPrefab;
        if (finalPrefab == null)
        {
            Debug.LogError($"Nenhum prefab final encontrado para o objeto: {currentObject.name}");
            return;
        }

        // Instanciar o prefab final antes de destruir o atual
        GameObject finalObject = Instantiate(finalPrefab, displayPosition.position, displayPosition.rotation, transform);
        finalObject.tag = "Pickup"; // Configurar o prefab como interativo

        // Definir o nome do objeto final como o nome do prefab final
        finalObject.name = finalPrefab.name;

        Debug.Log($"Objeto final instanciado: {finalObject.name}");

        // Apenas agora destruir o objeto atual
        Destroy(currentObject);
        currentObject = finalObject;

        cookingProgress = 0f; // Resetar progresso
    }
}


    private void ValidateMappings()
    {
        var duplicates = new HashSet<string>();
        foreach (var mapping in cookPrefabMappings)
        {
            string key = NormalizeKey(mapping.originalName);
            if (duplicates.Contains(key))
            {
                Debug.LogError($"Duplicação encontrada no mapeamento: {mapping.originalName}");
            }
            else
            {
                duplicates.Add(key);
            }
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

        // Obter o prefab intermediário
        string key = NormalizeKey(playerObject.name);
        if (!cookPrefabDictionary.TryGetValue(key, out var prefabs))
        {
            Debug.LogError($"Nenhum mapeamento encontrado para o objeto: {playerObject.name}");
            return false;
        }

        GameObject cookedPrefab = prefabs.cookedPrefab;
        if (cookedPrefab == null)
        {
            Debug.LogError($"O mapeamento para {playerObject.name} não possui um prefab intermediário.");
            return false;
        }

        // Instanciar o prefab intermediário
        currentObject = Instantiate(cookedPrefab, displayPosition.position, displayPosition.rotation, transform);
        currentObject.tag = "Pickup"; // Certifique-se de que seja interativo

        Debug.Log($"Objeto {playerObject.name} transformado no fogão para {currentObject.name}.");

        // Desativar o objeto do jogador
        playerObject.SetActive(false);

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



    private bool IsObjectCookable(GameObject obj)
    {
        string objNameLower = NormalizeKey(obj.name);
        return cookPrefabDictionary.ContainsKey(objNameLower);
    }

    private bool IsAlreadyCooked(GameObject obj)
    {
        return NormalizeKey(obj.name).Contains("_cooked");
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


    private GameObject GetCookedPrefab(string originalName)
{
    string key = originalName.ToLower();
    if (cookPrefabDictionary.TryGetValue(key, out var prefabs))
    {
        return prefabs.cookedPrefab;
    }
    return null;
}

private string NormalizeKey(string originalName)
    {
        // Remove "(Clone)" e transforma em letras minúsculas
        return originalName.Replace("(Clone)", "").Trim().ToLower();
    }
}
