using System.Text.Json.Serialization;
using Client.Identity.Models;
using ParrotWings.Models.Dto.Identity;

namespace Client.Helper;

[JsonSerializable(typeof(UserInfo))]
public partial class JsonContext : JsonSerializerContext;

[JsonSerializable(typeof(RegisterRequestDto))]
public partial class RegisterRequestDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(LoginRequestDto))]
public partial class LoginRequestDtoJsonContext : JsonSerializerContext;