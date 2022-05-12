using System.Collections.Generic;
using System.IO;
using System.Linq;
using HSE_Finance_App_Backend.Core;
using HSEProjectAppBackend.Context;
using NUnit.Framework;
using HSEProjectAppBackend.Context.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace HSEProjectAppBackend.UTesting
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            ApplicationContext context = new ApplicationContext();
            List<User> users = context.Users.ToList();

            Assert.AreEqual(1, users.Count);
            Assert.Pass();
        }

        [Test]
        public void TestLogin()
        {
            var response = UserTools.LoginUser("test_user", "test_password");
            var res = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            Assert.AreEqual(res["Status"], "True");
        }

        [Test]
        public void TestCompany()
        {
            ApplicationContext context = new ApplicationContext();

            var companies = context.Companies.ToList();

            Assert.IsNotNull(companies[0].Symbol, "A");
        }

        [Test]
        public void TestBSParse()
        {
            ApplicationContext context = new ApplicationContext();

            var sheets = context.BalanceSheets.ToList();

            Assert.AreEqual(sheets[0].Symbol, "A");
        }

        [Test]
        public void TestBSC()
        {
            ApplicationContext context = new ApplicationContext();

            var companies = context.Metrics.ToList();

            Assert.AreEqual(companies[0].PE, 30.72);
        }

        [Test]
        public void TestRelationship()
        {
            ApplicationContext context = new ApplicationContext();

            var companies = context.Companies.ToList();
            var balance_sheets = context.BalanceSheets.ToList();

            Assert.AreEqual(balance_sheets[0].Symbol, "A");
            Assert.AreEqual(companies[0].BalanceSheet.Symbol, "A");
        }

        [Test]
        public void TestSerialization()
        {
            ApplicationContext context = new ApplicationContext();

            var companies = context.Companies.ToList();
            var balance_sheets = context.BalanceSheets.ToList();
            var income_statements = context.IncomeStatements.ToList();
            var cash_flows = context.CashFlows.ToList();
            var metrics = context.Metrics.ToList();

            File.WriteAllText("deserialization.txt", companies[0].ToJson());
            Assert.IsTrue(true);
        }
    }
}