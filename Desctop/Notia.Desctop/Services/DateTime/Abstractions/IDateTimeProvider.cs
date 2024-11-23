
using System;

namespace Notia.Desctop.Services.DateTime.Abstractions;

internal interface IDateTimeProvider
{
    public DateTimeOffset CurrentTime();
}
