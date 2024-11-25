using System;

public class ProgressChangedEventArgs : EventArgs
{
    public float ProgressNormalized { get; }

    public ProgressChangedEventArgs(float progressNormalized)
    {
        ProgressNormalized = progressNormalized;
    }
}
