using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.EducationSystemServices
{
    using System.Composition;

    using Windows.Web.Http;
    using Windows.Web.Http.Filters;

    [Export(typeof(IEducationSystemService))]
    public class EducationSystemService : IEducationSystemService
    {
        public EducationSystemService()
        {

        }

        private async void Test()
        {
            var filter = new HttpBaseProtocolFilter();
            var http = new HttpClient(filter);
            var req = new HttpRequestMessage(HttpMethod.Get, new Uri("uri"));

            var result = await http.SendRequestAsync(req);

            // consume the response
            var responseText = result.StatusCode.ToString() + await result.Content.ReadAsStringAsync();

            // get the cookies of the request that elicited this response and display
            var responseCookies =
                filter.CookieManager.GetCookies(result.RequestMessage.RequestUri).Cast<HttpCookie>().ToArray();

            string cookiesString = string.Empty;
            for (int i = 0; i < responseCookies.Length; i++)
            {
                cookiesString = cookiesString + responseCookies[i].Name + "=" + responseCookies[i].Value
                                + Environment.NewLine;
            }

            // check to see the response cookies
            var responseCookiesString = cookiesString == string.Empty ? "Cookie collection is empty." : cookiesString;
        }
    }
}
