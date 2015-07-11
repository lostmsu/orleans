namespace UnitTests.General
{
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Orleans.Runtime.Configuration;
    using Orleans.Runtime.Host;

    [TestClass]
    public class PartialTrustSiloTests
    {
        [TestMethod]
        public void CanConfigureSiloHost()
        {
            var sandbox = RunTestSandbox(ConfigurePartialTrustSilo);
            AppDomain.Unload(sandbox);
        }

        static AppDomain RunTestSandbox(AppDomainInitializer initializer)
        {
            var permissions = GetPartialTrustPermissions();
            var applicationBase = Path.GetDirectoryName(typeof(PartialTrustSiloTests).Assembly.Location);

            string configFile = Path.Combine(applicationBase, "OrleansConfigurationForPartialTrustTesting.xml");
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

        static void ConfigurePartialTrustSilo(string[] args)
        {
            string siloName = "PartialTrustSilo";

            var siloHost = new SiloHost(siloName)
            {
                Config = new ClusterConfiguration(dynamicDefaults: false),
                ConfigFileName = args[0],
            };

            siloHost.LoadOrleansConfig();
        }
    }
}
