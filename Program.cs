namespace chatGptDiscordBot
{
    using Discord;
    using Discord.WebSocket;



    using System;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    internal class Program
    {
        DiscordSocketClient client;
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();



        private async Task MainAsync()
        {

            //discord
            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.Log += Log;


            var token = "Your token";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            Console.ReadLine();





        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task CommandsHandler(SocketMessage msg)
        {
            string apiKey = "your api key";
            string endpoint = "https://api.openai.com/v1/chat/completions";
            List<Message> messages = new List<Message>();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");





            var content = msg.Content;

            if (!msg.Author.IsBot)
            {
                var message = new Message() { Role = "user", Content = content };
                messages.Add(message);

                var requestData = new Request()
                {
                    ModelId = "gpt-3.5-turbo",
                    Messages = messages
                };
                using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);

                if (!response.IsSuccessStatusCode)
                {
                    msg.Channel.SendMessageAsync($"{(int)response.StatusCode} {response.StatusCode}");

                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseData>();
                var choices = responseData?.Choices ?? new List<Choice>();

                if (choices.Count == 0)
                {
                    msg.Channel.SendMessageAsync("No choices were returned by the API");

                }

                var choice = choices[0];
                var responseMessage = choice.Message;
                messages.Add(responseMessage);
                var responseText = responseMessage.Content.Trim();

                msg.Channel.SendMessageAsync($"ChatGPT: {responseText}");



            }


        }



    }
}