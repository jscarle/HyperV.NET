using System;

namespace HyperV.Definitions
{
    ///<summary>Defines the Security settings.</summary>
    public class SecurityDefinition
    {
        private bool encryptTraffic;

        ///<summary>Encrypt state and virtual machine migration traffic.</summary>
        public bool EncryptTraffic
        {
            get { return encryptTraffic; }
            set
            {
                if (value)
                {
                    TrustedPlatformModule = true;
                    encryptTraffic = true;
                }
                else
                {
                    encryptTraffic = false;
                }
            }
        }

        ///<summary>Enable Secure Boot.</summary>
        public bool SecureBoot { get; set; }
        private Guid secureBootTemplate;

        ///<summary>The template to use when Sceure Boot is enabled.</summary>
        public Guid SecureBootTemplate
        {
            get { return secureBootTemplate; }
            set
            {
                SecureBoot = true;
                secureBootTemplate = value;
            }
        }

        private bool shielding;

        ///<summary>Enable Shielding.</summary>
        public bool Shielding
        {
            get { return shielding; }
            set
            {
                if (value)
                {
                    SecureBoot = true;
                    TrustedPlatformModule = true;
                    encryptTraffic = true;
                    shielding = true;
                }
                else
                {
                    shielding = false;
                }
            }
        }

        ///<summary>Enable Trusted Platform Module.</summary>
        public bool TrustedPlatformModule { get; set; }

        public SecurityDefinition()
        {
            SecureBoot = true;
            SecureBootTemplate = HyperV.SecureBootTemplate.MicrosoftWindows;
        }
    }
}