
using System;

namespace Notia.Desctop.Exceptions;

internal class NotRegisteredPage : Exception
{
    public NotRegisteredPage(Type pageType) : base($"Page {pageType.Name} is not registered in app router")
    {
        PageType = pageType;
    }

    public Type PageType { get; }
}
