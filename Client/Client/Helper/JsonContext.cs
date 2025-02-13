using System.Text.Json.Serialization;
using Client.Models.Transactions;
using ParrotWings.Models.Dto.Identity;
using ParrotWings.Models.Dto.Transactions.GetTransactions;
using ParrotWings.Models.Dto.Users;

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

[JsonSerializable(typeof(GetTransactionsResponseDto))]
public partial class GetTransactionsResponseDtoJsonContext : JsonSerializerContext;

[JsonSerializable(typeof(CreateTransactionRequest))]
public partial class CreateTransactionRequestJsonContext : JsonSerializerContext;