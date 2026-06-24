namespace BusinessLogic.RabbitMQ;

public record ProductDeletionMessage(Guid ProductID, string? ProductName);
