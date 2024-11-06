using System;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    // Eventos para notificar sobre alterações de progresso e conclusão do progresso
    public Action<double> OnProgressChanged;  // Ação para quando o progresso muda
    public Action OnProgressComplete;         // Ação para quando o progresso é concluído

    [SerializeField] private bool resetOnComplete = false;
    [SerializeField, Range(0f, 1f)] private double progress = 0d;

    private double totalWork = 1d;

    public double Progress => progress;
    public double TotalWork => totalWork;
    public bool IsComplete => progress >= 1f;

    // Define o progresso como um valor entre 0 e 1
    public void SetProgress(double newProgress)
    {
        // Limita o progresso entre 0 e 1
        newProgress = Math.Clamp(newProgress, 0d, 1d);

        double oldProgress = this.progress;
        if (Math.Abs(oldProgress - newProgress) < 0.0001) return; // Retorna se o progresso não mudou

        this.progress = newProgress;

        // Invoca o evento de alteração de progresso
        OnProgressChanged?.Invoke(newProgress);

        // Verifica se o progresso está completo
        if (IsComplete)
        {
            OnProgressComplete?.Invoke(); // Invoca o evento de conclusão
            if (resetOnComplete) ResetProgress();
        }
    }

    public void SetTotalWork(double totalWork, double workDone = 0d)
    {
        this.totalWork = totalWork;
        SetWorkDone(workDone);
    }

    public void SetWorkDone(double workDone)
    {
        SetProgress(workDone / totalWork);
    }

    public void AddWorkDone(double workDone)
    {
        SetWorkDone(GetWorkDone() + workDone);
    }

    public double GetWorkDone() => progress * totalWork;
    public double GetWorkRemaining() => totalWork - GetWorkDone();

    public void ResetProgress() => SetProgress(0d);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        SetProgress(progress);
    }
#endif
}
