#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;

public static async Task Run(TimerInfo myTimer, ILogger log)
{
    var jsonString = await StackOverflowRequest();
    var jsonObj = JsonConvert.DeserializeObject<dynamic>(jsonString);

    var newQuestionCount = jsonObj.items.Count;

    await SlackRequest($"You have {newQuestionCount} questions on Stack Overflow");
    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
}

public static async Task<string> StackOverflowRequest()
{
    var epochTime = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    var subject = "typescript";

    HttpClientHandler handler = new HttpClientHandler()
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };

    using(var client = new HttpClient(handler))
    {
        var response = await client.GetAsync($"https://api.stackexchange.com/2.3/search?fromdate={epochTime}&order=desc&sort=activity&intitle={subject}&site=stackoverflow");
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }
}

public static async Task<string> SlackRequest(string message)
{
    using(var client = new HttpClient())
    {
        var requestData = new StringContent("{'text': '" + message + "'}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"https://hooks.slack.com/services/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", requestData);
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }
}