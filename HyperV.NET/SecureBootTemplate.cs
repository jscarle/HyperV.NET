using System;

namespace HyperV
{
    ///<summary>Defines the Secure Boot templates.</summary>
    public static class SecureBootTemplate
    {
        ///<summary>The Microsoft Windows template.</summary>
        public static Guid MicrosoftWindows { get; } = new Guid("1734C6E8-3154-4DDA-BA5F-A874CC483422");

        ///<summary>The Microsoft UEFI Certificate Authority template.</summary>
        public static Guid MicrosoftUEFICertificateAuthority { get; } = new Guid("272E7447-90A4-4563-A4B9-8E4AB00526CE");

        ///<summary>The Open Source Shielded VM template.</summary>
        public static Guid OpenSourceShieldedVM { get; } = new Guid("4292AE2B-EE2C-42B5-A969-DD8F8689F6F3");
    }
}