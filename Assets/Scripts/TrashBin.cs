using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("Configurações da Lixeira")]
    [SerializeField] private Transform playerHandTransform;

    public void DiscardObject(GameObject objectToDiscard)
    {
        if (objectToDiscard == null)
        {
            Debug.LogWarning("Nenhum objeto para descartar.");
            return;
        }

        // Verificar se o objeto é um prato
        if (objectToDiscard.TryGetComponent<Plate>(out Plate plate))
        {
            // Limpar o prato (remover todos os filhos)
            CleanPlate(plate);
        }
        else
        {
            // Destruir o objeto diretamente
            Destroy(objectToDiscard);
            Debug.Log($"Objeto {objectToDiscard.name} descartado na lixeira.");
        }
    }

    private void CleanPlate(Plate plate)
    {
        if (plate == null) return;

        List<Transform> childrenToRemove = new List<Transform>();

        // Adicionar todos os filhos do prato à lista para remoção
        foreach (Transform child in plate.transform)
        {
            childrenToRemove.Add(child);
        }

        // Remover e destruir todos os filhos
        foreach (Transform child in childrenToRemove)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"Prato {plate.name} foi limpo.");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("entrou trigger lixeira");
        if (playerHandTransform == null)
        {
            Debug.LogError("Transform da mão do jogador não configurado na lixeira.");
            return;
        }

        // Verificar se o objeto na mão do jogador pode ser descartado
        if (other.transform == playerHandTransform && playerHandTransform.childCount > 0)
        {
            // Descarta o primeiro objeto na mão do jogador
            foreach (Transform child in playerHandTransform)
            {
                DiscardObject(child.gameObject);
            }
        }
    }
}
