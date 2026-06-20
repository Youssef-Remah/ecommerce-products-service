namespace BusinessLogic.RabbitMQ;

public record ProductNameUpdateMessage(Guid ProductID, string? NewName);
