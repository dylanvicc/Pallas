namespace Pallas.API.Services.Hash
{
    public interface IPasswordHashService
    {
        string Hash(string password);

        bool Verify(string password, string store);
    }
}
