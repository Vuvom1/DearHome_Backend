using System;

namespace DearHome_Backend.Config;

public class NatsConfig
{
    public string Url { get; set; } = "nats://0.0.0.0:4222";
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "u1SGfGsTgAjuR3S62zEnu4055aTMssqC";
    public int ConnectionTimeout { get; set; } = 2000;
}