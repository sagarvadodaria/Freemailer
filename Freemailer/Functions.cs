using System;
using System.IO;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using RestSharp;

namespace Freemailer
{
    public class Functions
    {
        private const string EmailQ = "emailqueue";
        public static void ProcessQueueMessage([QueueTrigger(EmailQ)] EmailMessage emailMessage, TextWriter log)
        {
           var response = SendMessage(emailMessage);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                log.WriteLine(response.Content);
            }
        }

        public static IRestResponse SendMessage(EmailMessage emailMessage)
        {
            var key = CloudConfigurationManager.GetSetting("apiKey");
            var domain = CloudConfigurationManager.GetSetting("domain");

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", key);
                                             
            RestRequest request = new RestRequest();
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
                               
            request.Resource = "{domain}/messages";
            request.AddParameter("from", emailMessage.From);
            request.AddParameter("to", emailMessage.To);
            request.AddParameter("subject", emailMessage.Subject);
            request.AddParameter("text", emailMessage.Message);
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }
}
