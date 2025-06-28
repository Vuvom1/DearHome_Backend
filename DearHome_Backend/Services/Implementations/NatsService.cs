using System;
using NATS.Client;
using Microsoft.Extensions.Options;
using DearHome_Backend.Config;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DearHome_Backend.Services.Implementations;

public class NatsService : IDisposable
{
    private readonly IConnection _connection;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private bool _disposed;

    public NatsService(IOptions<NatsConfig> config)
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = config.Value.Url;
        // opts.Name = config.Value.Cluster;
        opts.Timeout = config.Value.ConnectionTimeout;

        if (!string.IsNullOrEmpty(config.Value.Username))
        {
            opts.User = config.Value.Username;
            opts.Password = config.Value.Password;
        }

        try
        {
            _connection = new ConnectionFactory().CreateConnection(opts);

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                MaxDepth = 64
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to connect to NATS server: {ex.Message}", ex);
        }
    }

    public IConnection GetConnection() => _connection;

    public void Publish(string subject, string message)
    {
        _connection.Publish(subject, System.Text.Encoding.UTF8.GetBytes(message));
    }

    public void Publish<T>(string subject, T data)
    {
        var jsonMessage = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        _connection.Publish(subject, System.Text.Encoding.UTF8.GetBytes(jsonMessage));
    }

    public IAsyncSubscription Subscribe(string subject, EventHandler<MsgHandlerEventArgs> handler)
    {
        return _connection.SubscribeAsync(subject, handler);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _connection?.Dispose();
        }

        _disposed = true;
    }
}
