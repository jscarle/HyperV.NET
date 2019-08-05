using System;

namespace HyperV
{
    public static class SecureBootTemplate
    {
        public static Guid MicrosoftWindows { get; } = new Guid("1734C6E8-3154-4DDA-BA5F-A874CC483422");
        public static Guid MicrosoftUEFICertificateAuthority { get; } = new Guid("272E7447-90A4-4563-A4B9-8E4AB00526CE");
        public static Guid OpenSourceShieldedVM { get; } = new Guid("4292AE2B-EE2C-42B5-A969-DD8F8689F6F3");
    }
}