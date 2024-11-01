﻿using Microsoft.Extensions.Logging;

namespace Adens.DevToys.Testbase.Mocks;
public class MockILogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return new MockIDisposable();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
    }
}
