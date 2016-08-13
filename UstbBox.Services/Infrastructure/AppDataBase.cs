using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services
{
    using System.IO;

    using LiteDB;

    using Microsoft.HockeyApp;

    internal class AppDataBase : LiteDatabase
    {
        private readonly ILog logger = HockeyLogManager.GetLog(typeof(AppDataBase));

        internal static AppDataBase CommonCache() => new AppDataBase(ServiceSettings.CacheDatabasePath);

        private AppDataBase(string connectionString)
            : base(connectionString)
        {
            this.Log.Logging += s => this.logger.Info(s);
        }
    }
}
