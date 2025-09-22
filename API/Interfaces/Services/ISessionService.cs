namespace API.Interfaces.Services;

public interface ISessionService
{
    void CreateSession(Guid userId);
    void DestroySession();
}