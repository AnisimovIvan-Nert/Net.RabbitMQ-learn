namespace Base;

public record ConnectionOptions(string ConnectionString, string Queue, bool Durable = false);