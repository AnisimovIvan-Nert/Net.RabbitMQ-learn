namespace Base.Receiver;

public record ReceiverOptions(ConnectionOptions ConnectionOptions, bool AutoAcknowledgement = true);