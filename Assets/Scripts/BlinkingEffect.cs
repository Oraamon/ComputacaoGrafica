using UnityEngine;
using System.Collections;

public class BlinkingEffect : MonoBehaviour
{
    public float blinkInterval = 0.5f;
    private Renderer objRenderer;
    private Color originalColor;
    private bool isBlinking = false;

    void Start()
    {
        // Obtenha o Renderer apenas do objeto atual
        objRenderer = GetComponent<Renderer>();

        if (objRenderer == null)
        {
            Debug.LogError("Nenhum Renderer encontrado no objeto: " + gameObject.name);
            return;
        }

        // Armazena a cor original do material
        originalColor = objRenderer.material.color;
    }

    public void StartBlinking()
    {
        if (objRenderer != null)
        {
            isBlinking = true;
            StartCoroutine(Blink());
            Debug.Log("Iniciando efeito de piscar no objeto: " + gameObject.name);
        }
    }

    public void StopBlinking()
    {
        if (objRenderer != null)
        {
            isBlinking = false;
            StopCoroutine(Blink());
            objRenderer.material.color = originalColor; // Restaura a cor original
            Debug.Log("Parando efeito de piscar no objeto: " + gameObject.name);
        }
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            objRenderer.material.color = Color.clear; // Define a cor transparente
            yield return new WaitForSeconds(blinkInterval);
            objRenderer.material.color = originalColor; // Restaura a cor original
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
