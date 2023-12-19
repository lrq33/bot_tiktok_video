using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

TelegramBotClient botclient = new TelegramBotClient("");

CancellationTokenSource cts = new() { };

ReceiverOptions rec = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};
botclient.StartReceiving
    (
        updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: rec,
    cancellationToken: cts.Token
    );
Console.WriteLine("Start bot...");
Console.ReadLine();
async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
{
    if (update.Message is not { } messgaae)
        return;
    if (messgaae.Text is not { } messagetext)
        return;
    var chat = messgaae.Chat.Id;
    Console.WriteLine($"You have received message from {chat} is {messagetext}");
    if (messagetext == "/start" || messagetext == "/Start")
    { await botclient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: "Send url of tiktok video that you waint to download"); }
    else
    {
        if (messagetext.Contains("https://www.tiktok.com") || messagetext.Contains("https://vm.tiktok.com/"))
        {
            Console.WriteLine("Start with web");
            HttpClient clientt = new HttpClient();
            HttpResponseMessage responsee = await clientt.GetAsync(messagetext);
            string pageSource = await responsee.Content.ReadAsStringAsync();
            // Extract video ID
            var regex = new Regex("\"id\":\"([^\"]+)\"");
            var match = regex.Match(pageSource);
            string videoId = match.Groups[1].Value;
            Console.WriteLine("Video ID: " + videoId);

            await botclient.SendVideoAsync(chatId: chat, video: InputFile.FromUri(await ks(videoId)));
        }
        else
        {
            await botclient.SendTextMessageAsync(chatId: chat, text: "ارسل رابط الفيديو في التيكتوك الصحيح   ");
        }
    }
}
Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
{
    Console.WriteLine("the probelm is:");
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;

}

string sem1 = "https://www.tiktok.com/@bruninho.oxl/video/7274268929890045189?is_from_webapp=1&sender_device=pc";
string sem2 = "https://vm.tiktok.com/ZMju5Cg3W/";
//var start = s.LastIndexOf("/") + 1;
//int end = s.IndexOf("?", start);
//string result = s.Substring(start,end  - start);
//Console.WriteLine($"{result}");
///-----------------------------------------------------------------------------------
//var d = s.Split('/');
//string de = d.Last();
//string result = de.Substring(0, 19);
//Console.WriteLine(result);

async Task<string> ks(string s)
{
    var client = new HttpClient();
    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/?aweme_id={s}");
    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
    var res = await response.Content.ReadAsStringAsync();
    //var jav = JsonConvert.DeserializeObject<dynamic>(res);
    var jase = JObject.Parse(res);
    JToken urlListToken = jase["aweme_list"][0]["video"]["play_addr"]["url_list"];

    // Convert the JToken to a list of URLs
    List<string> urlList = urlListToken.Select(url => url.ToString()).ToList();
    Console.WriteLine(urlList[0]);
    // Print the extracted URL list
    return urlList[0];
}