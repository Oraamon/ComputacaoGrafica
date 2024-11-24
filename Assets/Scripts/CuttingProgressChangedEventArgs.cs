using System;

public class CuttingProgressChangedEventArgs : EventArgs {
    public float ProgressNormalized { get; }

    public CuttingProgressChangedEventArgs(float progressNormalized) {
        ProgressNormalized = progressNormalized;
    }
}

