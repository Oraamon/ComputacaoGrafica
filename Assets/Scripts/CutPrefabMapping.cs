using System;
using UnityEngine;

// Classe para mapear o nome do item original ao prefab cortado
[Serializable]
public class CutPrefabMapping
{
    [Tooltip("Nome do objeto original que pode ser cortado.")]
    public string originalName;

    [Tooltip("Prefab do objeto cortado correspondente.")]
    public GameObject cutPrefab;
}
