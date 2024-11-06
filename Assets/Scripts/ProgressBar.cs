using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private ProgressTracker progressTracker;
    [SerializeField] private Image progressFillBar;
    [SerializeField] private GameObject progressBarUIObject;

    private void Start()
    {
        if (progressTracker != null)
        {
            progressTracker.OnProgressChanged += SetProgress;
            SetProgress(progressTracker.Progress);
        }
        else
        {
            Debug.LogWarning("ProgressTracker is null, progress will not be tracked.");
        }
    }

    public void SetProgress(double progress)
    {
        progressFillBar.fillAmount = (float)progress;
    }

    public void Show()
    {
        progressBarUIObject.SetActive(true);
    }

    public void Hide()
    {
        progressBarUIObject.SetActive(false);
    }
}
