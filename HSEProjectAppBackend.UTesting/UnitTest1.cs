using System.Collections.Generic;
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
        public void TestPortfolioParse()
        {
            ApplicationContext context = new ApplicationContext();

            var portfolios = context.Portfolios.ToList();

            Assert.AreEqual(portfolios[0].Uid, 1);
        }
    }
}