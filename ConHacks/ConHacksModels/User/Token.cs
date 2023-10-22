namespace ConHacksModels.User;

public class Token
{
    public Guid UserId { get; set; }
    public string TokenValue { get; set; }
    public DateTime Expiration { get; set; }
}