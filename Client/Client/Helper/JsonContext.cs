using System.Text.Json.Serialization;
using ParrotWings.Models.Dto.Identity;
using ParrotWings.Models.Dto.Users;
using ParrotWings.Models.Models;

namespace Client.Helper;

[JsonSerializable(typeof(RegisterRequestDto))]
public partial class RegisterRequestDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(LoginRequestDto))]
public partial class LoginRequestDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(TokenRefreshRequestDto))]
public partial class TokenRefreshRequestDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(TokenResponseDto))]
public partial class TokenResponseDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(GetUsersResponseDto))]
public partial class GetUsersResponseDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(UsersFilters))]
public partial class UsersFiltersJsonContext : JsonSerializerContext;