namespace SmartPark.Domain.Exceptions;

public class SpaceAlreadyOccupiedException : Exception
{
    public SpaceAlreadyOccupiedException(Guid id) : base($"Space {id} is already occupied.") { }
}