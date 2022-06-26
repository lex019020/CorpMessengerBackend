using System;

namespace CorpMessengerBackend.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime CurrentDateTime => DateTime.UtcNow;
    public DateTime MinValidTokenDateTime => DateTime.UtcNow.AddDays(-7);
}