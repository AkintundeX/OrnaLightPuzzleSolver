using Microsoft.Extensions.Logging;

namespace Logger;

public class LogWriter<T> : ILogWriter<T>
{
    private readonly ILogger<T> _logger;
    private readonly LogLevel _minimumLogLevel = LogLevel.Information;

    public LogWriter(LoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<T>();
#if DEBUG
        _minimumLogLevel = LogLevel.Trace;
#endif
    }

    public void Trace(string message, params object?[] args) => Trace(null, message, args);

    public void Trace(Exception? exception, string message, params object?[] args)
    {
        if (_minimumLogLevel == LogLevel.Trace)
        {
            _logger.LogTrace(exception, message, args);
        }
    }

    public void Debug(string message, params object[] args) => Debug(null, message, args);

    public void Debug(Exception? exception, string message, params object?[] args)
    {
        if (_minimumLogLevel < LogLevel.Debug)
            return;
        _logger.LogDebug(exception, message, args);
    }

    public void Information(string message, params object[] args) => Information(null, message, args);

    public void Information(Exception? exception, string message, params object?[] args)
    {
        if (_minimumLogLevel < LogLevel.Information)
            return;
        _logger.LogInformation(exception, message, args);
    }

    public void Warning(string message, params object?[] args) => Warning(null, message, args);

    public void Warning(Exception? exception, string message, params object?[] args)
    {
        if (_minimumLogLevel < LogLevel.Warning)
            return;
        _logger.LogWarning(exception, message, args);
    }

    public void Error(string message, params object?[] args) => Error(null, message, args);


    public void Error(Exception? exception, string message, params object?[] args)
    {
        _logger.LogError(exception, message, args);
    }
}