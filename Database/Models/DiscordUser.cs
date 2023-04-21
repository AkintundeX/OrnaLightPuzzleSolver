namespace Database.Models;

public class DiscordUser
{
    public string Name { get; set; }

    public DiscordUser(string name)
    {
        Name = name;
    }
}
