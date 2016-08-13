using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App
{
    using Windows.ApplicationModel.DataTransfer;

    using Reactive.Bindings;

    public static class UsualCommands
    {
        public static ReactiveCommand<string> CopyToClipboard { get; private set; }

        public static void Initialize()
        {
            CopyToClipboard = new ReactiveCommand<string>();
            CopyToClipboard.Subscribe(Copy);
        }

        private static void Copy(string copy)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(copy);
            Clipboard.SetContent(dataPackage);
        }
    }
}
