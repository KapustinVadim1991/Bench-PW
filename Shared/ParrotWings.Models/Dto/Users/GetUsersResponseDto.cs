namespace ParrotWings.Models.Dto.Users;

public class GetUsersResponseDto
{
    public IEnumerable<UserDto> Users { get; set; } = [];
}