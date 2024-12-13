namespace TheBench.Logic.Services;

public class IdService
{
    public string Generate(string type)
    {
        var guid = Guid.NewGuid();
        return $"{type}_{guid}";
    }
}