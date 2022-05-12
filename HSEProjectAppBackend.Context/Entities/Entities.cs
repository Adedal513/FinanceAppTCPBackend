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
}

[Table("companies")]
public class Company : Entity
{
    [Key] [JsonProperty] [Column("symbol")] public string Symbol { get; private set; }

    [JsonProperty] [Column("name")] public string Name { get; private set; }

    [JsonProperty] [Column("address")] public string Address { get; private set; }

    [JsonProperty] [Column("description")] public string Description { get; private set; }

    [JsonProperty] [Column("country")] public string Country { get; private set; }

    [JsonProperty] [Column("sector")] public string Sector { get; private set; }

    [JsonProperty] [Column("industry")] public string Industry { get; private set; }

    [JsonProperty] public BalanceSheet BalanceSheet { get; private set; }
    [JsonProperty] public IncomeStatement IncomeStatement { get; private set; }
    [JsonProperty] public CashFlow CashFlow { get; private set; }
    [JsonProperty] public Metrics Metrics { get; private set; }
}


[Table("balance_sheets")]
public class BalanceSheet : Entity
{
    [JsonProperty]
    [Key]
    [Column("symbol")]
    [ForeignKey("balance_sheets_symbol_fkey")]
    public string Symbol { get; private set; }

    [JsonProperty]
    [Column("reported_currency")]
    public string ReportedCurrency { get; private set; }

    [JsonProperty]
    [Column("total_current_assets")]
    public BigInteger TotalCurrentAssets { get; private set; }

    [JsonProperty]
    [Column("total_assets")]
    public BigInteger TotalAssets { get; private set; }

    [JsonProperty]
    [Column("total_current_liabilities")]
    public BigInteger TotalCurrentLiabilities { get; private set; }

    [JsonProperty]
    [Column("total_liabilities")]
    public BigInteger TotalLiabilities { get; private set; }

    [JsonProperty]
    [Column("common_stock")]
    public BigInteger CurrentStock { get; private set; }

    [JsonIgnore]
    public Company Company { get; private set; }
}

[Table("income_statements")]
public class IncomeStatement : Entity
{
    [JsonProperty]
    [Key]
    [Column("symbol")]
    public string Symbol { get; private set; }

    [JsonProperty] [Column("net_income")] public BigInteger? NetIncome { get; private set; }
    [JsonProperty] [Column("ibt")] public BigInteger? IBT { get; private set; }
    [JsonProperty] [Column("ebit")] public BigInteger? EBIT { get; private set; }
    [JsonProperty] [Column("ebitda")] public BigInteger? EBITDA { get; private set; }
    [JsonIgnore] public Company Company { get; private set; }
}

[Table("cash_flows")]
public class CashFlow : Entity
{
    [JsonProperty]
    [Key]
    [Column("symbol")] public string Symbol { get; private set; }

    [JsonProperty] [Column("operating_cash_flow")] public BigInteger? OperatingCF { get; private set; }
    [JsonProperty][Column("investment_cash_flow")] public BigInteger? InvestmentCF { get; private set; }
    [JsonProperty][Column("financing_cash_flow")] public BigInteger? FinancingCF { get; private set; }
    [JsonProperty][Column("change_in_cash_and_eq")] public BigInteger? ChangeInCF { get; private set; }
    [JsonIgnore] public Company Company { get; private set; }
}

[Table("ratios")]
public class Metrics : Entity
{
    [JsonProperty]
    [Key]
    [Column("symbol")] public string Symbol { get; private set; }

    [JsonProperty] [Column("market_cap")] public BigInteger MarketCap { get; private set; }
    [JsonProperty][Column("pe")] public double? PE { get; private set; }
    [JsonProperty][Column("dps")] public double? DPS { get; private set; }
    [JsonProperty][Column("dividend_yield")] public double? DividentYield { get; private set; }
    [JsonProperty][Column("eps")] public double? EPS { get; private set; }
    [JsonProperty][Column("rps")] public double? RPS { get; private set; }
    [JsonProperty][Column("roa")] public double? ROA { get; private set; }
    [JsonProperty][Column("roe")] public double? ROE { get; private set; }
    [JsonProperty][Column("beta")] public double? Beta { get; private set; }
    [JsonProperty][Column("evr")] public double? EVR { get; private set; }
    [JsonIgnore] public Company Company { get; private set; }
}