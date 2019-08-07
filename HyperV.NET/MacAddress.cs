using System;
using System.Text.RegularExpressions;

namespace HyperV
{
    ///<summary>Defines a MAC address.</summary>
    public class MacAddress
    {
        private readonly string macAddress;

        ///<summary>Initializes a new instance of the <see cref="MacAddress"/> class for the specified MAC address.</summary>
        ///<param name="macAddress">The MAC address.</param>
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

        ///<summary>Converts the current <see cref="MacAddress"/> object to its equivalent string representation.</summary>
        public override string ToString()
        {
            return ToString(false);
        }

        ///<summary>Converts the current <see cref="MacAddress"/> object to its equivalent string representation, optionally in lower case.</summary>
        ///<param name="lowercase"><c>true</c> for lower case; <c>false</c> for upper case.</param>
        public string ToString(bool lowercase)
        {
            if (lowercase)
                return macAddress.ToLower();
            else
                return macAddress;
        }

        ///<summary>Converts the current <see cref="MacAddress"/> object to its equivalent string representation using the specified separator.</summary>
        ///<param name="separator">A character that delimits the octects.</param>
        public string ToString(char separator)
        {
            return ToString(separator, false);
        }

        ///<summary>Converts the current <see cref="MacAddress"/> object to its equivalent string representation using the specified separator, optionally in lower case.</summary>
        ///<param name="separator">A character that delimits the octects.</param>
        ///<param name="lowercase"><c>true</c> for lower case; <c>false</c> for upper case.</param>
        public string ToString(char separator, bool lowercase)
        {
            if (lowercase)
                return $"{macAddress.Substring(0, 2).ToLower()}{separator}{macAddress.Substring(2, 2).ToLower()}{separator}{macAddress.Substring(4, 2).ToLower()}{separator}{macAddress.Substring(6, 2).ToLower()}{separator}{macAddress.Substring(8, 2).ToLower()}{separator}{macAddress.Substring(10, 2).ToLower()}";
            else
                return $"{macAddress.Substring(0, 2)}{separator}{macAddress.Substring(2, 2)}{separator}{macAddress.Substring(4, 2)}{separator}{macAddress.Substring(6, 2)}{separator}{macAddress.Substring(8, 2)}{separator}{macAddress.Substring(10, 2)}";
        }
    }
}