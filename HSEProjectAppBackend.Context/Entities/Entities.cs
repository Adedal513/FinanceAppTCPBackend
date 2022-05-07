using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace HSEProjectAppBackend.Context.Entities;

public abstract class Entity
{
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

[Table("users")]
public class User : Entity
{
    [JsonProperty] [Column("uid")] public int Id { get; set; }

    [JsonProperty] [Column("username")] public string Username { get; set; }

    [JsonProperty] [Column("password")] public string Password { get; set; }

    [JsonProperty] [NotMapped] public List<Portfolio> Portfolios { get; set; }
}

[Table("companies")]
public class Company : Entity
{
    [Key] [JsonProperty] public string Symbol { get; private set; }

    [JsonProperty] [Column("name")] public string Name { get; private set; }

    [JsonProperty] [Column("address")] public string Address { get; private set; }

    [JsonProperty] [Column("description")] public string Description { get; private set; }

    [JsonProperty] [Column("country")] public string Country { get; private set; }

    [JsonProperty] [Column("sector")] public string Sector { get; private set; }

    [JsonProperty] [Column("industry")] public string Industry { get; private set; }
}

[Table("portfolios")]
public class Portfolio
{
    [JsonProperty] [Key, Column("pid")] public int Pid { get; private set; }

    [JsonProperty] [Key, Column("uid")] [ForeignKey("users.uid")]  public int Uid { get; private set; }

    [JsonProperty] [Column("symbol")] public List<string> Companies { get; private set; }
}