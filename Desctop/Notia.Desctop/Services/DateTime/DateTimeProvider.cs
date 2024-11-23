
using Notia.Desctop.Services.DateTime.Abstractions;
using System;

namespace Notia.Desctop.Services.DateTime;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset CurrentTime() => DateTimeOffset.Now;  
}
