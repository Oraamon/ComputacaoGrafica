using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlinkingEffect : MonoBehaviour
{
    public float blinkInterval = 0.5f; // Intervalo de tempo para alternar o efeito
    public Color outlineColor = Color.white; // Cor da borda do outline
    public float outlineThickness = 0.1f; // Espessura da borda do outline

    private List<Renderer> objRenderers = new List<Renderer>();
    private Dictionary<Renderer, List<Material>> originalMaterials = new Dictionary<Renderer, List<Material>>();
    private Material outlineMaterial; // Material do shader de outline
    private bool isBlinking = false;

    void Start()
    {
        // Obter todos os componentes Renderer no objeto atual e seus filhos
        objRenderers.AddRange(GetComponentsInChildren<Renderer>());

        if (objRenderers.Count == 0)
        {
            Debug.LogError("Nenhum Renderer encontrado no objeto ou seus filhos: " + gameObject.name);
            return;
        }

        // Armazenar os materiais originais de todos os renderers
        foreach (var renderer in objRenderers)
        {
            originalMaterials[renderer] = new List<Material>(renderer.materials);
        }

        // Criar o material de outline usando o shader
        Shader outlineShader = Shader.Find("Custom/OutlinedShader");
        if (outlineShader == null)
        {
            Debug.LogError("OutlinedShader não encontrado na pasta Assets!");
            return;
        }

        outlineMaterial = new Material(outlineShader);
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_OutlineThickness", outlineThickness);
    }

    public void StartBlinking()
    {
        if (outlineMaterial == null)
        {
            Debug.LogError("Material de outline não criado!");
            return;
        }

        if (objRenderers.Count > 0)
        {
            isBlinking = true;
            StartCoroutine(Blink());
            Debug.Log("Iniciando o efeito de destaque no objeto: " + gameObject.name);
        }
    }

    public void StopBlinking()
    {
        if (objRenderers.Count > 0)
        {
            isBlinking = false;
            StopCoroutine(Blink());

            // Restaurar os materiais originais para cada renderer
            foreach (var renderer in objRenderers)
            {
                if (renderer != null)
                {
                    renderer.materials = originalMaterials[renderer].ToArray();
                }
            }
            Debug.Log("Parando o efeito de destaque no objeto: " + gameObject.name);
        }
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            // Adicionar o material de outline
            foreach (var renderer in objRenderers)
            {
                if (renderer != null)
                {
                    var materials = new List<Material>(renderer.materials);
                    if (!materials.Contains(outlineMaterial))
                    {
                        materials.Add(outlineMaterial);
                        renderer.materials = materials.ToArray();
                    }
                }
            }
            yield return new WaitForSeconds(blinkInterval);

            // Remover o material de outline
            foreach (var renderer in objRenderers)
            {
                if (renderer != null)
                {
                    var materials = new List<Material>(renderer.materials);
                    if (materials.Contains(outlineMaterial))
                    {
                        materials.Remove(outlineMaterial);
                        renderer.materials = materials.ToArray();
                    }
                }
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }

}
