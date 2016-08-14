using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.CredentialServices
{
    using Windows.Security.Credentials;

    using Microsoft.HockeyApp;

    using UstbBox.Core;

    public class CredentialService : ICredentialService
    {
        private readonly PasswordVault passwordValut = new PasswordVault();

        public void SaveCredential(PasswordCredential credential)
        {
            var saved = this.GetCredential(credential.Resource);
            if (saved != null) this.RemoveCredential(saved);
            this.passwordValut.Add(credential);
        }

        public void RemoveCredential(string kindId)
        {
            var credential = this.GetCredential(kindId);
            if (credential == null) return;
            this.RemoveCredential(credential);
        }

        public PasswordCredential GetCredential(string kindId)
        {
            try
            {
                return this.passwordValut.FindAllByResource(kindId).FirstOrDefault();
            }
            catch (Exception ex) when (ex.HResult == Constants.ElementNotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                HockeyClient.Current.TrackException(ex);
                return null;
            }
        }

        public IReadOnlyList<PasswordCredential> RetrieveAll()
        {
            return this.passwordValut.RetrieveAll();
        }

        public void RemoveCredential(PasswordCredential credential)
        {
            try
            {
                this.passwordValut.Remove(credential);
            }
            catch (Exception ex) when (ex.HResult == Constants.ElementNotFound)
            {
            }
            catch (Exception ex)
            {
                HockeyClient.Current.TrackException(ex);
            }
        }
    }
}
