namespace Client.Identity.Models;

/// <summary>
/// Response for login and registration.
/// </summary>
public class FormResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the action was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// On failure, the problem details are parsed and returned in this array.
    /// </summary>
    public string[] ErrorList { get; set; } = [];
}

public class FormResult<T>
{
    private T? _data;
    public T? Data
    {
        get => Succeeded ? _data : throw new InvalidOperationException("The action was not successful.");
        set => _data = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the action was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// On failure, the problem details are parsed and returned in this array.
    /// </summary>
    public string[] ErrorList { get; set; } = [];
}