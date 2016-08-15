using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services
{
    using LiteDB;
    using LiteDB.Platform;

    using UstbBox.Core;
    using UstbBox.Models.Images;

    public class ServicesBootstrapper
    {
        public void Run()
        {
            LitePlatform.Initialize(new LitePlatformWindowsStore());

            var mapper = BsonMapper.Global;

            mapper.Entity<ImageObject>()
                .Id(x => x.Name, false)
                .Index(x => x.Name, new IndexOptions() { IgnoreCase = true, Unique = true });
        }
    }
}
