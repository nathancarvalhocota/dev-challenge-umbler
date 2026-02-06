using Desafio.Umbler.Models;
using Desafio.Umbler.Repositories;
using Desafio.Umbler.Services;
using Desafio.Umbler.Services.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Desafio.Umbler.Test.Services
{
    [TestClass]
    public class DomainServiceTests
    {
        [TestMethod]
        public async Task DomainService_CacheHit_DoesNotCallExternalClients()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var domain = new Domain
            {
                Name = "test.com",
                Ip = "192.168.0.1",
                HostedAt = "umbler.corp",
                WhoIs = "whois-raw",
                UpdatedAt = DateTime.Now,
                Ttl = 9999
            };

            using var db = new DatabaseContext(options);
            db.Domains.Add(domain);
            db.SaveChanges();

            var repository = new DomainRepository(db);
            var whoisClient = new Mock<IWhoisClient>(MockBehavior.Strict);
            var dnsClient = new Mock<IDnsLookupClient>(MockBehavior.Strict);

            var service = new DomainService(repository, whoisClient.Object, dnsClient.Object);

            var result = await service.GetDomainInfoAsync("test.com");

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Status);
            whoisClient.VerifyNoOtherCalls();
            dnsClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DomainService_CacheExpired_CallsExternalAndUpdates()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var domain = new Domain
            {
                Name = "test.com",
                Ip = "192.168.0.1",
                HostedAt = "old",
                WhoIs = "old",
                UpdatedAt = DateTime.Now.AddMinutes(-10),
                Ttl = 1
            };

            using var db = new DatabaseContext(options);
            db.Domains.Add(domain);
            db.SaveChanges();

            var repository = new DomainRepository(db);
            var whoisClient = new Mock<IWhoisClient>();
            var dnsClient = new Mock<IDnsLookupClient>();

            dnsClient.Setup(c => c.QueryARecordAsync("test.com"))
                .ReturnsAsync(new DnsLookupResult("1.1.1.1", 120));

            whoisClient.Setup(c => c.QueryAsync("test.com"))
                .ReturnsAsync(new WhoisQueryResult("whois-raw", null));

            whoisClient.Setup(c => c.QueryAsync("1.1.1.1"))
                .ReturnsAsync(new WhoisQueryResult(string.Empty, "Org"));

            var service = new DomainService(repository, whoisClient.Object, dnsClient.Object);

            var result = await service.GetDomainInfoAsync("test.com");

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Status);
            dnsClient.Verify(c => c.QueryARecordAsync("test.com"), Times.Once);
            whoisClient.Verify(c => c.QueryAsync("test.com"), Times.Once);
            whoisClient.Verify(c => c.QueryAsync("1.1.1.1"), Times.Once);
        }

        [TestMethod]
        public async Task DomainService_NoARecord_Returns_NoARecord_And_Skips_Ip_Whois()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var domain = new Domain
            {
                Name = "test.com",
                Ip = null,
                HostedAt = null,
                WhoIs = "old",
                UpdatedAt = DateTime.Now.AddMinutes(-10),
                Ttl = 1
            };

            using var db = new DatabaseContext(options);
            db.Domains.Add(domain);
            db.SaveChanges();

            var repository = new DomainRepository(db);
            var whoisClient = new Mock<IWhoisClient>();
            var dnsClient = new Mock<IDnsLookupClient>();

            dnsClient.Setup(c => c.QueryARecordAsync("test.com"))
                .ReturnsAsync(new DnsLookupResult(null, 0));

            whoisClient.Setup(c => c.QueryAsync("test.com"))
                .ReturnsAsync(new WhoisQueryResult("whois-raw", null));

            var service = new DomainService(repository, whoisClient.Object, dnsClient.Object);

            var result = await service.GetDomainInfoAsync("test.com");

            Assert.IsNotNull(result);
            Assert.AreEqual("NO_A_RECORD", result.Status);
            dnsClient.Verify(c => c.QueryARecordAsync("test.com"), Times.Once);
            whoisClient.Verify(c => c.QueryAsync("test.com"), Times.Once);
            whoisClient.Verify(c => c.QueryAsync("1.1.1.1"), Times.Never);
        }

        [TestMethod]
        public async Task DomainService_InvalidDomain_Returns_Null_And_Skips_External()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var db = new DatabaseContext(options);
            var repository = new DomainRepository(db);
            var whoisClient = new Mock<IWhoisClient>(MockBehavior.Strict);
            var dnsClient = new Mock<IDnsLookupClient>(MockBehavior.Strict);

            var service = new DomainService(repository, whoisClient.Object, dnsClient.Object);

            var result = await service.GetDomainInfoAsync("127.0.0.1");

            Assert.IsNull(result);
            whoisClient.VerifyNoOtherCalls();
            dnsClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Domain_Moking_WhoisClient()
        {
            //arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var db = new DatabaseContext(options);
            var repository = new DomainRepository(db);
            var whoisClient = new Mock<IWhoisClient>();
            var dnsClient = new Mock<IDnsLookupClient>();

            dnsClient.Setup(c => c.QueryARecordAsync("test.com"))
                .ReturnsAsync(new DnsLookupResult("1.1.1.1", 120));

            whoisClient.Setup(c => c.QueryAsync("test.com"))
                .ReturnsAsync(new WhoisQueryResult("whois-raw", null));

            whoisClient.Setup(c => c.QueryAsync("1.1.1.1"))
                .ReturnsAsync(new WhoisQueryResult(string.Empty, "Org"));

            var service = new DomainService(repository, whoisClient.Object, dnsClient.Object);

            //act
            var result = await service.GetDomainInfoAsync("test.com");

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Status);
            whoisClient.Verify(c => c.QueryAsync("test.com"), Times.Once);
            whoisClient.Verify(c => c.QueryAsync("1.1.1.1"), Times.Once);
        }
    }
}
