namespace SommusProject.Services;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}