using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.Extensions
{
    public static class EncodingExtensions
    {
        public static string ReadGb2312(this byte[] bytes)
        {
            return CodePagesEncodingProvider.Instance.GetEncoding("gb2312").GetString(bytes);
        }
    }
}
