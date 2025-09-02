namespace TheBench.Logic.Config;

public record MailGunConfiguration(
    string BaseUri = "",
    string Domain = "",
    string ApiKey = ""
)
{
    public string ApiKey { get; private set; } = ApiKey;

    public void SetApiKey(string key) => ApiKey = key;
};