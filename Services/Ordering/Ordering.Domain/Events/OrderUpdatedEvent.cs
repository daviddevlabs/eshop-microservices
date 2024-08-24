namespace Ordering.Domain;

public record OrderUpdatedEvent(Order Order) : IDomainEvent;