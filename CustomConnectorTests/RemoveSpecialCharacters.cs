using Newtonsoft.Json.Linq;
using System.Net;

namespace CustomConnectorTests
{
    public class Script : ScriptBase
    {
        public override async Task<HttpResponseMessage> ExecuteAsync()
        {
            if (this.Context.OperationId == "RemoveSpecialCharacters")
            {
                return await this.HandleRemoveSpecialCharactersOperation().ConfigureAwait(false);
            }

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            response.Content = CreateJsonContent($"Unknown operation ID '{this.Context.OperationId}'");
            return response;
        }

        private async Task<HttpResponseMessage> HandleRemoveSpecialCharactersOperation()
        {
            HttpResponseMessage response;

            var contentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);

            var contentAsJson = JObject.Parse(contentAsString);

            var specialCharacters = contentAsJson["specialCharacters"].ToObject<string>();
            var text = contentAsJson["text"].ToObject<string>();

            var sanitizedText = RemoveSpecialCharacters(text, specialCharacters);

            JObject output = new JObject
            {
                ["sanitizedText"] = sanitizedText,
            };

            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = CreateJsonContent(output.ToString());
            return response;
        }

        public static string RemoveSpecialCharacters(string targetString, string specialChars)
        {
            foreach (char specialChar in specialChars)
            {
                targetString = targetString.Replace(specialChar.ToString(), "");
            }
            return targetString;
        }

        private HttpContent CreateJsonContent(string json)
        {
            return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }
    }
}