using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressBarUI : MonoBehaviour 
{
    [SerializeField] private MesaCorte cuttingCounter;
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject progressBarUIObject;

    private void Start() 
    {
        if (cuttingCounter != null)
        {
            cuttingCounter.OnProgressChanged += CuttingCounter_OnProgressChanged;
            cuttingCounter.OnCutComplete += CuttingCounter_OnCutComplete;
            cuttingCounter.OnCutPaused += CuttingCounter_OnCutPaused;
            cuttingCounter.OnCutCancelled += CuttingCounter_OnCutCancelled;
        }
        else
        {
            Debug.LogError("CuttingCounter não está atribuído no Inspector.");
        }
        Hide();
        if (barImage != null)
        {
            barImage.fillAmount = 0f;
        }
    }

    private void CuttingCounter_OnProgressChanged(object sender, CuttingProgressChangedEventArgs e) 
    { 
        if (barImage != null)
        {
            barImage.fillAmount = e.ProgressNormalized; 
            Show();
        }
        Debug.LogWarning("PROGRESS: " + barImage.fillAmount);
    }

    private void CuttingCounter_OnCutComplete(object sender, EventArgs e)
    {
        Hide();
    }

    private void CuttingCounter_OnCutPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void CuttingCounter_OnCutCancelled(object sender, EventArgs e)
    {
        Hide();
    }

    public void Show()
    {
        if (progressBarUIObject != null)
        {
            progressBarUIObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (progressBarUIObject != null)
        {
            progressBarUIObject.SetActive(false);
        }

        if (barImage != null)
        {
            barImage.fillAmount = 0f; 
        }
    }
}
