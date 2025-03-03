using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using alexm_app.Utils.TicTacToe;

namespace alexm_app.Services
{
    public static class ReportSender
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string WebHookUrl = @"https://discord.com/api/webhooks/1344018570899750962/2UjcMyjYGfNijD8xIBChB6MZBzJIaHrW4mRV5D_z2pDIrGLn_yQREFA5TYUUVr4Ms4jF";
        public static async Task SendMessage(string message)
        {
            string jsonMessage = $"{{\"content\": \"{message}\"}}";
            StringContent content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(WebHookUrl, content);

            }
            catch
            {
                Debug.WriteLine("Webhook issue");
            }
        }
        public static Button GetReportButton()
        {
            Button button = new Button() { Text = "Send a report"};
            button.Clicked += Button_Clicked;
            return button;
        }

        private static async void Button_Clicked(object? sender, EventArgs e)
        {
            string response = await Shell.Current.CurrentPage.DisplayPromptAsync("Report", "Send a report");
            if(response != "")
            {
                await SendMessage(response);
            }
        }
    }
}
