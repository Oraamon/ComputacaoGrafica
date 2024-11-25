using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private MonoBehaviour progressibleObject;
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject progressBarUIObject;

    private IProgressible progressible;

    private void Start()
    {
        if (progressibleObject is IProgressible progressible)
        {
            this.progressible = progressible;
            this.progressible.OnProgressChanged += Progressible_OnProgressChanged;
            this.progressible.OnProgressComplete += Progressible_OnProgressComplete;

            if (progressibleObject is MesaCorte mesaCorte)
            {
                mesaCorte.OnCutPaused += CuttingCounter_OnCutPaused;
                mesaCorte.OnCutCancelled += CuttingCounter_OnCutCancelled;
            }
        }
        else
        {
            Debug.LogError("O objeto atribuído não implementa IProgressible.");
        }
        Hide();
        if (barImage != null)
        {
            barImage.fillAmount = 0f;
        }
    }

    private void OnDestroy()
    {
        if (progressible != null)
        {
            progressible.OnProgressChanged -= Progressible_OnProgressChanged;
            progressible.OnProgressComplete -= Progressible_OnProgressComplete;

            if (progressibleObject is MesaCorte mesaCorte)
            {
                mesaCorte.OnCutPaused -= CuttingCounter_OnCutPaused;
                mesaCorte.OnCutCancelled -= CuttingCounter_OnCutCancelled;
            }
        }
    }

    private void Progressible_OnProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (barImage != null)
        {
            barImage.fillAmount = e.ProgressNormalized;
            Show();
        }
    }

    private void Progressible_OnProgressComplete(object sender, EventArgs e)
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
