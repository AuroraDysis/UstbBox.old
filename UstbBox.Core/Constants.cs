using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Core
{
    public class Constants
    {
        /// <summary>
        /// The credential retrieval functions raise an "Element not found" exception if there were no matches.
        /// The credential retrieval functions raise an "Element not found" exception if the credential is no longer in the PasswordVault. (For example, if the user manually removed the credential via the Control Panel.)
        /// </summary>
        public const int ElementNotFound = unchecked((int)0x80070490);
    }
}
