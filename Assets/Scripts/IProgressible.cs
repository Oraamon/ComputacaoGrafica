using System;

public interface IProgressible
{
    event EventHandler<ProgressChangedEventArgs> OnProgressChanged;
    event EventHandler OnProgressComplete;
    float GetProgressNormalized(); // Retorna o progresso normalizado
}
