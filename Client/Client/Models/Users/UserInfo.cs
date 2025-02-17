﻿namespace Client.Models.Users;

/// <summary>
/// User info from identity endpoint to establish claims.
/// </summary>

public record UserInfo(
    string Id,
    string Email,
    string Name
);