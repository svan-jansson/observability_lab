using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoadSimulator
{
    class Program
    {
        const string ApiEndpoint = "https://localhost:5001";
        public static int Parallelism = 4;

        public static string[] ValidCurrencCodes = new[]
        {
            "SEK",
            "EUR",
            "USD",
            "GBP",
            "DKK",
            "NOK",
            "CNY",
            "JPY",
            "EGP",
            "BRL"
        };

        public static string[] InvalidCurrencCodes = new[]
        {
            "ISK",
            "INR",
            "CHF",
            "RUB"
        };

        public static Random Rng = new Random();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Parallelism = Convert.ToInt32(args[0]);
            }

            Console.WriteLine($"LoadSimulator: Using parallelism {Parallelism}.");

            // Wait until servies are ready...
            Task.Delay(5_000).Wait();

            while (true)
            {
                var tasks = new List<Task>();
                for (var i = 0; i < Parallelism; i++)
                {
                    var validCurrencyCodes = PickMany(ValidCurrencCodes);
                    var invalidCurrencyCodes = Array.Empty<string>();
                    var from = Pick(ValidCurrencCodes);
                    var value = Rng.Next(0, 10_000) + 1;

                    if (HappensEvery(nthOccation: 100))
                    {
                        invalidCurrencyCodes = PickMany(InvalidCurrencCodes);
                    }

                    if (HappensEvery(nthOccation: 100))
                    {
                        from = Pick(InvalidCurrencCodes);
                    }

                    var to = validCurrencyCodes.Concat(invalidCurrencyCodes).ToArray();

                    var task = CallCurrencyConverter(from, value, to);
                    tasks.Add(task);

                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private static Task<HttpResponseMessage> CallCurrencyConverter(string from, int value, string[] to)
        {
            var httpClient = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(ApiEndpoint + $"/CurrencyConverter/{from}?value={value}"),
                Content = new StringContent(JsonConvert.SerializeObject(to), Encoding.UTF8, "application/json")
            };

            return httpClient.SendAsync(request);
        }

        private static bool HappensEvery(int nthOccation)
        {
            return (Rng.Next(nthOccation) == nthOccation - 1);
        }

        private static T Pick<T>(params T[] values)
        {
            var max = values.Length;
            var value = values[Rng.Next(0, max)];
            return value;
        }

        private static T[] PickMany<T>(T[] list)
        {
            var sorted = list.OrderBy(n => Guid.NewGuid()).ToArray();
            var max = list.Length;
            var selection = list.Take(Rng.Next(1, max));
            return selection.ToArray();
        }
    }
}
