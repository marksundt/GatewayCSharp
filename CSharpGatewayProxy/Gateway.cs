using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml;

namespace CSharpGatewayAPIDemo
{

    public struct config
    {
        public const string ApiKey = "API Key Here";
        public const string JsonMediaType = "application/json";
        public const string BaseUri = "https://gateway-sb.clearent.net";
        public const string TransactionUri = "rest/v2/transactions";
    }

    public class JsonTransaction
    {
        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }
        [JsonProperty(PropertyName = "card")]
        public string card { get; set; }
        [JsonProperty(PropertyName = "exp-date")]
        public string expDate { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public string amount { get; set; }
    }

public class JsonResponse
    {
        [JsonProperty(PropertyName = "code")]
        public string code { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string status { get; set; }
        [JsonProperty(PropertyName = "payload")]
        public string payload { get; set; }
    }


    //http://json2csharp.com/# 
    public class Transaction2
    {
        public string amount { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string result { get; set; }
        public string card { get; set; }
        [JsonProperty(PropertyName = "authorization-code")]
        public string authorizationCode { get; set; }
        [JsonProperty(PropertyName = "batch-string-id")]
        public string batchStringId { get; set; }
        [JsonProperty(PropertyName = "display-message")]
        public string displayMessage { get; set; }
        [JsonProperty(PropertyName = "result-code")]
        public string resultCode { get; set; }
        [JsonProperty(PropertyName = "exp-date")]
        public string expDate { get; set; }
}

public class Payload
    {
        public Transaction2 transaction { get; set; }
        public string payloadType { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string id { get; set; }
    }

    public class RootObject
    {
        public string code { get; set; }
        public string status { get; set; }
        [JsonProperty(PropertyName = "exchange-id")]
        public string exchangeId { get; set; }
        public Payload payload { get; set; }
        public List<Link> links { get; set; }
    }
class Gateway
    {
        static void Main()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(config.BaseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(config.JsonMediaType));
            client.DefaultRequestHeaders.Add("api-key", config.ApiKey);

            string transaction;

            //Dictionary to JSON
            //Dictionary<string, string> Transaction = new Dictionary<string, string>();
            //Transaction.Add("type", "SALE");
            //Transaction.Add("card", "4111111111111111");
            //Transaction.Add("exp-date", "1219");
            //Transaction.Add("amount", "25.00");
            //transaction = JsonConvert.SerializeObject(Transaction);

            JsonTransaction jTransaction = new JsonTransaction();

            //C# Annotated object to JSON  
            jTransaction.type = "SALE";
            jTransaction.card = "4111111111111111";
            jTransaction.expDate = "1219";
            jTransaction.amount = "25.00";
            string jsonTrans = JsonConvert.SerializeObject(jTransaction); 

            // sale
           // transaction = "{\"type\":\"SALE\",\"card\":\"4111111111111111\", \"exp-date\":\"1219\",\"amount\":\"25.00\"}";

            //JsonTransaction testResp = JsonConvert.DeserializeObject<JsonTransaction>(transaction);

            RunTransaction(client, jsonTrans).Wait();
           
            // authorization
            transaction = "{\"type\":\"AUTH\",\"card\":\"4111111111111111\", \"exp-date\":\"1219\",\"amount\":\"25.00\"}";
            RunTransaction(client, transaction).Wait();

            //// capture
            //transaction = "{\"type\":\"CAPTURE\",\"amount\":\"25.00\",\"id\":\"146523\"}";
            //RunTransactionAsync(client, transaction).Wait();
            
            //// forced sale
            //transaction = "{\"type\":\"FORCED SALE\",\"card\":\"4111111111111111\", \"exp-date\":\"1219\",\"amount\":\"25.00\",\"order-id\":\"000123456\",\"authorization-code\":\"APPC10\"}";
            //RunTransactionAsync(client, transaction).Wait();
            
            //// refund
            //transaction = "{\"type\":\"REFUND\",\"amount\":\"25.00\",\"id\":\"12345\"}";
            //RunTransactionAsync(client, transaction).Wait();
            
            //// void
            //transaction = "{\"type\":\"VOID\",\"id\":\"12345\"}";
            //RunTransactionAsync(client, transaction).Wait();
            
        }


        static async Task<string> RunTransaction(HttpClient client, string transaction)
        {
            // examine our request body
            Debug.WriteLine(new string('-', 40));
            Debug.WriteLine("transaction = " + transaction);

            // convert this to http content
            var content = new StringContent(transaction);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            // send our request and wait for response
            HttpResponseMessage response = await client.PostAsync(config.TransactionUri, content);

            // examine the response
            Debug.WriteLine("response = " + response.Content.ReadAsStringAsync().Result);

            var foo = response.Content.ReadAsStringAsync().Result;

            RootObject jResp = JsonConvert.DeserializeObject<RootObject>(foo);

            Debug.WriteLine("Responce Code = " + jResp.code);

            return foo;
        }
    }

}