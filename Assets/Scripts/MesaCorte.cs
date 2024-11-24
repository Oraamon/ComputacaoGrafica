using System;
using System.Collections.Generic;
using UnityEngine;

public class MesaCorte : MonoBehaviour 
{
    [SerializeField] private float cuttingTime = 2f; // Tempo necessário para cortar o item
    [SerializeField] private List<string> cuttableNames; // Lista de nomes dos objetos cortáveis
    [SerializeField] private Transform itemPlace; // Ponto onde os itens são colocados

    [Header("Cut Prefab Mappings")]
    [Tooltip("Lista que mapeia o nome do item original ao seu prefab cortado.")]
    [SerializeField] private List<CutPrefabMapping> cutPrefabMappings; // Mapeamento de objetos cortados

    private Dictionary<string, GameObject> cutPrefabDictionary; // Dicionário para mapeamento rápido
    private Dictionary<string, float> itemCuttingProgress; // Dicionário para salvar progresso de corte

    private GameObject currentObject; // Objeto atual na mesa
    private float cuttingProgress = 0f; // Progresso do corte
    private bool isCutting = false; // Indica se o corte está ativo

    // Eventos agora usam OnProgressChangedEventArgs
    public event EventHandler<CuttingProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCutComplete; // Evento ao completar o corte
    public event EventHandler OnCutPaused; // Evento ao pausar o corte
    public event EventHandler OnCutCancelled; // Evento ao cancelar o corte

    private void Start()
    {
        if (itemPlace == null)
        {
            Debug.LogError("ItemPlace não está atribuído no Inspector.");
        }

        if (cutPrefabMappings == null || cutPrefabMappings.Count == 0)
        {
            Debug.LogError("CutPrefabMappings não estão configuradas no Inspector.");
        }

        // Inicializar o dicionário para mapeamento rápido
        cutPrefabDictionary = new Dictionary<string, GameObject>();
        foreach (CutPrefabMapping mapping in cutPrefabMappings)
        {
            string key = mapping.originalName.ToLower();
            if (!cutPrefabDictionary.ContainsKey(key))
            {
                cutPrefabDictionary.Add(key, mapping.cutPrefab);
            }
            else
            {
                Debug.LogWarning($"Duplicação encontrada no mapeamento: {mapping.originalName}. Apenas o primeiro mapeamento será usado.");
            }
        }

        // Inicializar o dicionário para progresso de corte
        itemCuttingProgress = new Dictionary<string, float>();
    }

    private void Update() 
    {
        if (currentObject == null || !isCutting) return;

        cuttingProgress += Time.deltaTime;

        // Atualizar o progresso no dicionário
        string key = currentObject.name.ToLower();
        if (itemCuttingProgress.ContainsKey(key))
        {
            itemCuttingProgress[key] = cuttingProgress / cuttingTime;
        }
        else
        {
            itemCuttingProgress.Add(key, cuttingProgress / cuttingTime);
        }

        // Dispara evento com OnProgressChangedEventArgs
        OnProgressChanged?.Invoke(this, new CuttingProgressChangedEventArgs(cuttingProgress / cuttingTime));

        if (cuttingProgress >= cuttingTime) 
        {
            CompleteCut();
        }
    }

    private void CompleteCut() 
    {
        if (currentObject != null) 
        {
            Debug.Log($"Objeto {currentObject.name} cortado!");
            Destroy(currentObject); // Remove o objeto original

            // Encontrar o prefab cortado correspondente
            GameObject cutPrefab = GetCutPrefab(currentObject.name);

            if (cutPrefab != null)
            {
                // Instanciar o prefab cortado na posição e rotação especificadas
                GameObject cutObject = Instantiate(cutPrefab, itemPlace.position, itemPlace.rotation, transform);
                cutObject.tag = "Pickup"; // Define a tag para o objeto cortado, se necessário

                Debug.Log($"Objeto cortado instanciado: {cutObject.name}");

                // Atualizar a referência para o objeto cortado
                currentObject = cutObject;

                // Remover o progresso salvo, já que o corte foi concluído
                string key = cutObject.name.ToLower();
                if (itemCuttingProgress.ContainsKey(key))
                {
                    itemCuttingProgress.Remove(key);
                }
            }
            else
            {
                Debug.LogError($"Não foi encontrado um prefab cortado para o objeto: {currentObject.name}");
                currentObject = null;
            }

            isCutting = false;
            cuttingProgress = 0f;

            OnCutComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TryPlaceObject(GameObject playerObject) 
    {
        if (currentObject != null) 
        {
            Debug.LogWarning("A mesa de corte já está ocupada.");
            return false; // A mesa já está ocupada
        }
        if (!IsObjectCuttable(playerObject)) 
        {
            Debug.LogWarning($"O objeto {playerObject.name} não é cortável.");
            return false; // O item não é cortável
        }

        // Verificar se o item já foi cortado
        if (IsAlreadyCut(playerObject))
        {
            Debug.LogWarning($"O objeto {playerObject.name} já foi cortado e não pode ser cortado novamente.");
            return false;
        }

        currentObject = playerObject;
        currentObject.transform.position = itemPlace.position;
        currentObject.transform.rotation = itemPlace.rotation;
        currentObject.transform.parent = itemPlace; // Define o parent para itemPlace

        // Ativar o objeto se estava desativado
        currentObject.SetActive(true);

        Debug.Log($"Objeto {currentObject.name} colocado na mesa de corte.");

        // Inicializar o progresso de corte para este item, se ainda não existir
        string key = currentObject.name.ToLower();
        if (!itemCuttingProgress.ContainsKey(key))
        {
            itemCuttingProgress.Add(key, 0f);
        }
        else
        {
            cuttingProgress = itemCuttingProgress[key] * cuttingTime;
            Debug.Log($"Progresso de corte para {currentObject.name} restaurado: {itemCuttingProgress[key] * 100}%");
        }

        return true;
    }

    public GameObject TakeObject() 
    {
        if (currentObject == null) return null;

        GameObject objectToTake = currentObject;
        currentObject = null;
        isCutting = false;
        cuttingProgress = 0f;

        objectToTake.transform.parent = null;
        objectToTake.SetActive(true);

        Debug.Log($"Objeto {objectToTake.name} retirado da mesa de corte.");

        return objectToTake;
    }

    public void StartCutting() 
    {
        if (currentObject == null)
        {
            Debug.LogWarning("Não há nenhum objeto na mesa de corte para cortar.");
            return;
        }

        // Verificar se o objeto já foi cortado
        if (IsAlreadyCut(currentObject))
        {
            Debug.LogWarning($"O objeto {currentObject.name} já foi cortado e não pode ser cortado novamente.");
            return;
        }

        if (isCutting)
        {
            Debug.LogWarning("O corte já está em andamento.");
            return;
        }

        isCutting = true;
        Debug.Log("Iniciando o corte do objeto na mesa de corte.");
    }

    public void PauseCutting()
    {
        if (isCutting)
        {
            isCutting = false;
            Debug.Log("Corte pausado.");
            OnCutPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public void CancelCutting()
    {
        if (isCutting)
        {
            isCutting = false;
            cuttingProgress = 0f;
            Debug.Log("Corte cancelado.");
            OnCutCancelled?.Invoke(this, EventArgs.Empty);

            // Salvar o progresso atual no dicionário
            string key = currentObject.name.ToLower();
            if (itemCuttingProgress.ContainsKey(key))
            {
                itemCuttingProgress[key] = cuttingProgress / cuttingTime;
            }
            else
            {
                itemCuttingProgress.Add(key, cuttingProgress / cuttingTime);
            }

            // Resetar o progresso de corte
            cuttingProgress = 0f;
        }
    }

    private bool IsObjectCuttable(GameObject obj) 
    {
        foreach (string name in cuttableNames)
        {
            if (string.Equals(obj.name, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAlreadyCut(GameObject obj)
    {
        // Verifica se o objeto já foi cortado com base no nome
        // Aqui, assumimos que objetos cortados possuem "_cut" no nome
        return obj.name.ToLower().Contains("_cut");
    }

    // Método para obter o prefab cortado correspondente
    private GameObject GetCutPrefab(string originalName)
    {
        foreach (CutPrefabMapping mapping in cutPrefabMappings)
        {
            if (string.Equals(mapping.originalName, originalName, StringComparison.OrdinalIgnoreCase))
            {
                return mapping.cutPrefab;
            }
        }
        return null; // Retorna null se não encontrar
    }
}


