using System;

namespace HyperV.Definitions
{
    public class SecurityDefinition
    {
        private bool encryptTraffic;

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

        public bool SecureBoot { get; set; }
        private Guid secureBootTemplate;

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

        public bool TrustedPlatformModule { get; set; }

        public SecurityDefinition()
        {
            SecureBoot = true;
            SecureBootTemplate = HyperV.SecureBootTemplate.MicrosoftWindows;
        }
    }
}