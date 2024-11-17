using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlinkingEffect : MonoBehaviour
{
    public float blinkInterval = 0.5f;
    private List<Renderer> objRenderers = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
    private bool isBlinking = false;

    void Start()
    {
        // Get all Renderer components in the current object and its children
        objRenderers.AddRange(GetComponentsInChildren<Renderer>());

        if (objRenderers.Count == 0)
        {
            Debug.LogError("No Renderers found on the object or its children: " + gameObject.name);
            return;
        }

        // Store the original colors of all materials
        foreach (var renderer in objRenderers)
        {
            originalColors[renderer] = renderer.material.color;
        }
    }

    public void StartBlinking()
    {
        if (objRenderers.Count > 0)
        {
            isBlinking = true;
            StartCoroutine(Blink());
            Debug.Log("Starting blink effect on object: " + gameObject.name);
        }
    }

    public void StopBlinking()
    {
        if (objRenderers.Count > 0)
        {
            isBlinking = false;
            StopCoroutine(Blink());

            // Restore original colors for each renderer
            foreach (var renderer in objRenderers)
            {
                if (renderer != null)
                {
                    renderer.material.color = originalColors[renderer];
                }
            }
            Debug.Log("Stopping blink effect on object: " + gameObject.name);
        }
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            // Set all renderers to transparent
            foreach (var renderer in objRenderers)
            {
                renderer.material.color = Color.clear;
            }
            yield return new WaitForSeconds(blinkInterval);

            // Restore original colors
            foreach (var renderer in objRenderers)
            {
                renderer.material.color = originalColors[renderer];
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
