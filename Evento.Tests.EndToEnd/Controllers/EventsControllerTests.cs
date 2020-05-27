using Autofac.Extensions.DependencyInjection;
using Evento.Infrastructure.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Evento.Tests.EndToEnd.Controllers
{
    public class EventsControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public EventsControllerTests()
        {
            _server = new TestServer(
                new WebHostBuilder()
                .ConfigureAppConfiguration(a => a.AddJsonFile("appsettings.Development.json"))
                .ConfigureServices(x => x.AddAutofac())
                .UseStartup<Startup>()
            );
            _client = _server.CreateClient();
        }


        [Fact]
        public async Task fetching_events_should_return_not_empty_collection()
        {
            var response = await _client.GetAsync("event");
            var content = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<IEnumerable<EventDto>>(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            events.Should().NotBeEmpty();
        }
    }
}
