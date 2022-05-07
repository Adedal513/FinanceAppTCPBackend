using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
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

    [JsonProperty] [NotMapped] public BalanceSheet BalanceSheet { get; private set; } 
}

[Table("portfolios")]
public class Portfolio : Entity
{
    [JsonProperty] [Key, Column("pid")] public int Pid { get; private set; }

    [JsonProperty] [Key, Column("uid")] [ForeignKey("users.uid")]  public int Uid { get; private set; }

    [JsonProperty] [Column("symbol")] public List<string> Companies { get; private set; }
}

public class BalanceSheet : Entity
{
    [JsonProperty] [Key, Column("symbol")] public string Symbol { get; private set; }

    [JsonIgnore] public Company Company { get; private set; }

    [JsonProperty] [Column("reported_currency")] public string ReportedCurrency { get; private set; }

    [JsonProperty] [Column("total_current_assets")] public BigInteger TotalCurrentAssets { get; private set; }
     
    [JsonProperty] [Column("total_assets")] public BigInteger TotalAssets { get; private set; }

    [JsonProperty] [Column("total_current_liabilities")] public BigInteger TotalCurrentLiabilities { get; private set; }

    [JsonProperty] [Column("total_liabilities")] public BigInteger TotalLiabilities { get; private set; }

    [JsonProperty] [Column("common_stock")] public BigInteger CurrentStock { get; private set; }


}