using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Model
{
    public static class Networking
    {
        /*
 * Handling of HTTP connection
 */
        private static HttpClient client = new HttpClient(GetInsecureHandler());

        //Dealing with self-signed cert on emulator
        private static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };
            return handler;
        }

        //GETting
        public static async Task<string> GetResponse(string URL)
        {
            string reply = "";

            var response = await client.GetAsync(URL);
            if (response.IsSuccessStatusCode)
            {
                reply = await response.Content.ReadAsStringAsync();
            }
            else
            {
                reply = "HTTPErr";
            }

            return reply;
        }
    }
}
