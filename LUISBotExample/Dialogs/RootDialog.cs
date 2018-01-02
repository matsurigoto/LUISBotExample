using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestSharp;
using System.Collections.Generic;

namespace LUISBotExample.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private string key = "your_key";

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var client = new RestClient("https://westus.api.cognitive.microsoft.com/luis/v2.0/");
            var request = new RestRequest("apps/your_app_id", Method.GET);
            request.AddParameter("subscription-key", key);
            request.AddParameter("verbose", "true");
            request.AddParameter("timezoneOffset", "0");
            request.AddParameter("q", activity.Text);

            var response = await client.ExecuteTaskAsync<RootObject>(request);

            var suggestion = string.Empty;
            foreach (var item in response.Data.intents)
            {
                suggestion += item.intent + ":" + item.score.ToString() + "<BR/>";
            }
            await context.PostAsync($"{suggestion}");

            context.Wait(MessageReceivedAsync);
        }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class RootObject
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<object> entities { get; set; }
    }
}