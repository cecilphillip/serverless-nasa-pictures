using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EmojiData;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Converters;
using Twilio.Rest.Api.V2010.Account;

namespace NasaPOD
{
    public class NasaPOD
    {
        private readonly IConfiguration config;
        private readonly ILogger log;
        private readonly IHttpClientFactory clientFactory;

        public NasaPOD(ILogger<NasaPOD> log, IConfiguration config, IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            this.log = log;
            this.config = config;
        }

        [FunctionName("NasaPOD")]
        public async Task Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer)
        {
            var nasaClient = clientFactory.CreateClient("nasa");

            // Send request to NASA POD API
            var nasaAPIKey = config["NASA_API_KEY"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"planetary/apod?api_key={nasaAPIKey}");

            var response = await nasaClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var jsonDocument = await JsonDocument.ParseAsync(responseStream);
                var jsonRoot = jsonDocument.RootElement;

                var mediaUrl = new Uri(jsonRoot.GetProperty("url").GetString());
                var title = jsonRoot.GetProperty("title").GetString();


                //  Send SMS via Twilio
                string accountSid = config["TWILIO_ACCOUNT_SID"];
                string authToken = config["TWILIO_AUTH_TOKEN"];

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: $"Check out this video of: {title}! \n\n  {Emoji.KnownEmoji.Rocket}",
                    mediaUrl: Promoter.ListOfOne(mediaUrl),
                    from: new Twilio.Types.PhoneNumber(config["MY_TWILIO_NUMBER"]),
                    to: new Twilio.Types.PhoneNumber(config["RECEIVER_NUMBER"])
                );
            }
            else
            {                
                log.LogError($"Request to NASA POD API Failed with Status Code {response.StatusCode}");
            }
        }
    }
}
