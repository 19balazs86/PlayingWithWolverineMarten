namespace Worker.Ping.Messages;

public record LocalPingMessage(int Counter);

public record LocalPongMessage(int Counter);