using ReactiveUI;
using System;

namespace Notia.Desctop.ViewModels;

public class ViewModelBase : ReactiveObject, IDisposable
{
    public virtual void Dispose()
    {
    }
}
