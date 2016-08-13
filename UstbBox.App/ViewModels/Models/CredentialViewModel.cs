using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels.Models
{
    using Windows.Security.Credentials;

    using UstbBox.Models.Credentials;

    public class CredentialViewModel
    {
        public PasswordCredential Credential { get; set; }

        public CredentialKind Kind { get; set; }
    }
}
