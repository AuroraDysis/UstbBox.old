using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.TeachServices
{
    using Flurl;
    using Flurl.Http;

    using HtmlAgilityPack;

    using UstbBox.Core.Extensions;
    using UstbBox.Models.Teach;
    using UstbBox.Services.Extensions;

    public class TeachHelper
    {
        public async Task<List<TeachNewsItem>> GetLatestNews()
        {
            var ret = await "http://teach.ustb.edu.cn/".EnableCookies().GetBytesAsync().ConfigureAwait(false);
            var doc = new HtmlDocument();
            var text = ret.ReadGb2312();
            var html = System.Net.WebUtility.HtmlDecode(text);

            doc.LoadHtml(html);

            var col = doc.DocumentNode.Descendants("table")
                .Where(
                    x =>
                        x.Attributes.Count == 4 && x.GetAttributeValue("cellspacing", null) == "0" &&
                        x.GetAttributeValue("cellpadding", null) == "0" && x.GetAttributeValue("width", null) == "100%" &&
                        x.GetAttributeValue("border", null) == "0")
                .Skip(2).Take(8);

            var newsRaw = col.Select(x => x.Element("tbody").Element("tr").Descendants("td").ToList()).ToList();

            var result =
                newsRaw.Select(
                    raw =>
                    new TeachNewsItem
                        {
                            Link =
                                Url.Combine(
                                    "http://teach.ustb.edu.cn/",
                                    raw.First().Element("a").GetAttributeValue("href", null)),
                            Name = raw.First().Element("a").InnerText,
                            Date = raw.Last().Element("font").InnerText
                        }).ToList();

            return result;
        }

        public void SaveNewsItems(IEnumerable<TeachNewsItem> items)
        {
            using (var db = AppDatabase.CommonCache())
            {
                var col = db.GetCollection<TeachNewsItem>("TeachNews");
                col.Insert(items);
            }
        }
    }
}
