using System;

namespace CorpMessengerBackend.Services;

public interface IDateTimeService
{
    public DateTime CurrentDateTime { get; }
    public DateTime MinValidTokenDateTime { get; }
}