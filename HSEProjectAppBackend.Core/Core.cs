﻿using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using HSEProjectAppBackend.Context;
using HSEProjectAppBackend.Context.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HSE_Finance_App_Backend.Core;

public interface IEntityTools
{
    static readonly IConfigurationBuilder Builder;
}

public class UserTools : IEntityTools
{
    private static readonly IConfigurationBuilder Builder =
        new ConfigurationBuilder().AddJsonFile(@"Config\UserToolsSettings.json");

    public string LoginUser(string username, string password)
    {
        var response = new Dictionary<string, string>();

        using (var context = new ApplicationContext())
        {
            if (context.Users.Any(x => x.Username == username && x.Password == password))
                response.Add("Status", "True");
            else
                response.Add("Status", "False");
        }

        return ResponseBuilder.Builder(response);
    }

    public string RegisterUser(User NewUser)
    {
        var root = Builder.Build();

        var response = new Dictionary<string, string>();

        using (var context = new ApplicationContext())
        {
            if (context.Users.Any(x => x.Username == NewUser.Username))
            {
                response.Add("Status", "False");
                response.Add("ResponseCode", "1");
                response.Add("Comment", root["1"]);
            }
            else
            {
                context.Users.Add(NewUser);
                context.SaveChanges();

                response.Add("Status", "True");
            }
        }

        return ResponseBuilder.Builder(response);
    }
}

public class CompanyTools : IEntityTools
{
    private static readonly IConfigurationBuilder Builder =
        new ConfigurationBuilder().AddJsonFile(@"Configuration\CompanyToolsSettings.json");
    public List<Company> companies;
    public List<BalanceSheet> balances;
    public List<IncomeStatement> incomes;
    public List<CashFlow> cashflows;
    public List<Metrics> metrics;

    public CompanyTools()
    {
        using (var context = new ApplicationContext())
        {
            companies = context.Companies.ToList();
            balances = context.BalanceSheets.ToList();
            incomes = context.IncomeStatements.ToList();
            cashflows = context.CashFlows.ToList();
            metrics = context.Metrics.ToList();
        }
    }

    public string GetCompany(string symbol)
    {
        var response = new Dictionary<string, string>();

        var company = this.companies.FirstOrDefault(x => x.Symbol == symbol);

        if (company != null)
        {
            response.Add("Status", "True");
            response.Add("Company", company.ToJson());

            return ResponseBuilder.Builder(response);
        }

        response.Add("Status", "False");

        return ResponseBuilder.Builder(response);
    }

    public string GetCompaniesWithOffset(int offset, int amount)
    {
        var response = new Dictionary<string, string>();

        var res_companies = this.companies.Skip(offset - 1).Take(amount).ToList();

        return JsonConvert.SerializeObject(res_companies);
    }
}

public class ResponseBuilder
{
    public static string Builder(Dictionary<string, string> response)
    {
        var json = JsonConvert.SerializeObject(response);

        return json;
    }
}

public class Execute
{
    public Execute()
    {

    }

    public string ExecuteRequest(string request)
    {
        if (request == null)
            return null;
        
        var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
        var requestType = requestDict["FUNCTION"];
        string response;

        switch (requestType)
        {
            case "GetCompany":
            {
                var companies = new CompanyTools();

                return companies.GetCompany(requestDict["SYMBOL"]);
            }
            case "GetCompanyWithOffset":
            {
                var offset = int.Parse(requestDict["OFFSET"]);
                var amount = int.Parse(requestDict["AMOUNT"]);

                var companies = new CompanyTools();

                return companies.GetCompaniesWithOffset(offset, amount);
            }
            case "Login":
            {
                var login = requestDict["LOGIN"];
                var password = requestDict["PASSWORD"];

                var users = new UserTools();

                return users.LoginUser(login, password);
            }
            case "Register":
            {
                var login = requestDict["LOGIN"];
                var password = requestDict["PASSWORD"];

                var users = new UserTools();
                var newUser = new User()
                {
                    Username = login,
                    Password = password
                };

                return users.RegisterUser(newUser);
            }
            default:
            {
                return null;
            }
        }

    }
}