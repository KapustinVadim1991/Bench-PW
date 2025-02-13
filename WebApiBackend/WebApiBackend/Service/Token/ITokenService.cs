using WebApiBackend.Database.Domain;

namespace WebApiBackend.Service.Token;

public interface ITokenService
{
    string GenerateJwtToken(
        AppUser user
    );

    RefreshToken GenerateRefreshToken(string ipAddress);
}