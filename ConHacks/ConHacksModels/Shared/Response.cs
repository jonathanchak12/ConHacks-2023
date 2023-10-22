namespace ConHacksModels.Shared;

public class Response
{
    public bool? IsSuccess { get; set; } = true;
    public object? Result { get; set; }
    public string? DisplayMessage { get; set; } = "";
    public List<string>? ErrorMessages { get; set; }
}

public class UserResponse
{
    public string Status { get; set; } = "";
    public string Message { get; set; } = "";
    public bool Success { get; set; }
}