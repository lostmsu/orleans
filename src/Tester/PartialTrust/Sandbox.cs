/*
Project Orleans Cloud Service SDK ver. 1.0
 
Copyright (c) Microsoft Corporation
 
All rights reserved.
 
MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the ""Software""), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace UnitTests.PartialTrust
{
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    public static class Sandbox
    {
        public const string PartialTrustConfig = "OrleansConfigurationForPartialTrustTesting.xml";

        public static readonly string ConfigFullPath =
            Path.Combine(
                Path.GetDirectoryName(typeof(Sandbox).Assembly.Location),
                PartialTrustConfig);

        public static AppDomain Run(AppDomainInitializer initializer)
        {
            var permissions = GetPartialTrustPermissions();
            var applicationBase = Path.GetDirectoryName(typeof(SiloTests).Assembly.Location);

            string configFile = Path.Combine(applicationBase, PartialTrustConfig);
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, configFile));

            string logFile = Path.Combine(applicationBase, "PartialTrustTest.log");
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, logFile));

            var sandboxSetup = new AppDomainSetup
            {
                AppDomainInitializer = initializer,
                ApplicationBase = applicationBase,
                AppDomainInitializerArguments = new[] { configFile },
            };

            var hostDomain = AppDomain.CreateDomain("OrleansHost", null, sandboxSetup, permissions);
            return hostDomain;
        }

        static PermissionSet GetPartialTrustPermissions()
        {
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
            return SecurityManager.GetStandardSandbox(evidence);
        }
    }
}
