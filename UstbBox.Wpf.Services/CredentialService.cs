namespace UstbBox.Wpf.Services
{
    using System.Collections.Generic;
    using CredentialManagement;
    using Models;

    public class CredentialService
    {
        private CredentialService()
        {
        }

        public static CredentialService Instance { get; } = new CredentialService();

        public void SaveCredential(Credential credential)
        {
            credential.PersistanceType = PersistanceType.LocalComputer;
            credential.Save();
        }

        public void DeleteCredential(string id)
        {
            var credential = new Credential();
            credential.Target = id;
            credential.Delete();
        }

        public Credential GetCredential(string id)
        {
            var credential = new Credential();
            credential.Target = id;
            return credential.Load() ? credential : null;
        }

        public IEnumerable<Credential> RetrieveAll()
        {
            foreach (var item in CredentialKind.AllKindIds)
            {
                var credential = this.GetCredential(item);
                if (credential != null)
                {
                    yield return credential;
                }
            }
        }
    }
}
