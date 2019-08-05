using System;
using System.Text.RegularExpressions;

namespace HyperV
{
    public class MacAddress
    {
        private readonly string macAddress;

        public MacAddress(string macAddress)
        {
            string sanitizedMacAddress = Regex.Replace(macAddress.ToUpper(), "[^0-9A-F]", "");

            if (sanitizedMacAddress.Length != 12)
                throw new ArgumentException($"{macAddress} is not a valid MAC address.");

            byte firstOctect = Convert.ToByte(sanitizedMacAddress.Substring(0, 2), 16); ;
            if ((firstOctect & (1 << 0)) != 0)
                throw new ArgumentException($"{macAddress} is not a unicast MAC address.");

            this.macAddress = sanitizedMacAddress;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool lowercase)
        {
            if (lowercase)
                return macAddress.ToLower();
            else
                return macAddress;
        }

        public string ToString(char separator)
        {
            return ToString(separator, false);
        }

        public string ToString(char separator, bool lowercase)
        {
            if (lowercase)
                return $"{macAddress.Substring(0, 2).ToLower()}{separator}{macAddress.Substring(2, 2).ToLower()}{separator}{macAddress.Substring(4, 2).ToLower()}{separator}{macAddress.Substring(6, 2).ToLower()}{separator}{macAddress.Substring(8, 2).ToLower()}{separator}{macAddress.Substring(10, 2).ToLower()}";
            else
                return $"{macAddress.Substring(0, 2)}{separator}{macAddress.Substring(2, 2)}{separator}{macAddress.Substring(4, 2)}{separator}{macAddress.Substring(6, 2)}{separator}{macAddress.Substring(8, 2)}{separator}{macAddress.Substring(10, 2)}";
        }
    }
}