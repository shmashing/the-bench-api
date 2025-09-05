namespace TheBench.Logic.Config;

public record MailGunConfiguration(
    string BaseUri = "",
    string Domain = "",
    string ApiKey = ""
);