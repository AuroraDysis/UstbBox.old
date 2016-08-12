using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services
{
    using System.Composition;

    using LiteDB;
    using LiteDB.Platform;

    using UstbBox.Core;

    [Export(nameof(ServicesBootstrapper), typeof(IBootstrapper))]
    public class ServicesBootstrapper : IBootstrapper
    {
        public async Task Run()
        {
            LitePlatform.Initialize(new LitePlatformWindowsStore());
        }
    }
}
