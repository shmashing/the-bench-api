namespace TheBench.Logic.Config;

public record ApiConfiguration(
    string BaseUiUri,
    string DatabaseConnectionString,
    MailGunConfiguration MailGunConfiguration
    );