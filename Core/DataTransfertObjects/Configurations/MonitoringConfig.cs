namespace DataTransfertObjects.Configurations;

public record MonitoringConfig
{
    public string ServerId { get; set; }
    public string BaseUrl { get; set; }
    public int HeartbeatIntervalSeconds { get; set; } = 30;
}
