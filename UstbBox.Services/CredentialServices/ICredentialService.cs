using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.CredentialServices
{
    using Windows.Security.Credentials;

    public interface ICredentialService
    {
        void SaveCredential(PasswordCredential credential);

        void RemoveCredential(string kindId);

        void RemoveCredential(PasswordCredential credential);

        PasswordCredential GetCredential(string kindId);

        IReadOnlyList<PasswordCredential> RetrieveAll();
    }
}
