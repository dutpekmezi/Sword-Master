using System;

public abstract class BaseSystem : IDisposable
{
    protected bool _isDisposed;

    protected BaseSystem()
    {
        OnInitialize();
    }

    protected virtual void OnInitialize() { }
    protected virtual void OnDispose() { }
    public virtual void Tick() { }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        OnDispose();
    }
}
