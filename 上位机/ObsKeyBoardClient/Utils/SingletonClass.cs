using System;

namespace ObsKeyBoardClient.Utils;

public class SingletonClass<T> : IDisposable where T : class, new()
{
    private static readonly Lazy<T> _instance = new(() => new T());
    protected readonly object Lock = new();

    protected bool IsDisposed;

    public static T Instance => _instance.Value;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SingletonClass()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (!IsDisposed)
        {
            if (isDisposing)
            {
            }

            IsDisposed = true;
        }
    }
}