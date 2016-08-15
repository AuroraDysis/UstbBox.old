using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services
{
    using System.IO;

    using Windows.Storage;

    using LiteDB;

    using Microsoft.HockeyApp;

    internal class AppDatabase : LiteDatabase
    {
        private readonly ILog logger = HockeyLogManager.GetLog(typeof(AppDatabase));

        internal static AppDatabase CommonCache()
            => new AppDatabase("common_cache.db");

        private AppDatabase(string connectionString)
            : base(connectionString)
        {
            this.Log.Logging += s => this.logger.Info(s);
        }
    }
}
