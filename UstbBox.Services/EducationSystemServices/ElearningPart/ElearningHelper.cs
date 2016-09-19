using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.EducationSystemServices.ElearningPart
{
    using Windows.Web.Http;
    using Windows.Web.Http.Filters;

    public class ElearningHelper
    {
        private HttpBaseProtocolFilter filter;

        private HttpClient client = new HttpClient();

        public async Task SignIn()
        {
            this.filter = new HttpBaseProtocolFilter();
            this.client = new HttpClient(this.filter);
        }
    }
}
