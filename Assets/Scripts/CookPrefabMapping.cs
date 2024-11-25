using System;
using UnityEngine;

[Serializable]
public class CookPrefabMapping
{
    [Tooltip("Nome do item original (case-insensitive).")]
    public string originalName;
    
    [Tooltip("Prefab do item após o cozimento.")]
    public GameObject cookedPrefab;
}
