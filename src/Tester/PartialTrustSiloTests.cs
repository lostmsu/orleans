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
        public void CanStartSiloHostInPartialTrust()
        {
            var permissions = GetPartialTrustPermissions();
            var applicationBase = Path.GetDirectoryName(typeof(PartialTrustSiloTests).Assembly.Location);

            string configFile = Path.Combine(applicationBase, "OrleansConfigurationForTesting.xml");
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, configFile));

            var hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitPartialTrustSilo,
                ApplicationBase = applicationBase,
                AppDomainInitializerArguments = new[] { configFile },
            }, permissions);

            try
            {

            }
            finally
            {
                AppDomain.Unload(hostDomain);
            }
            Assert.Inconclusive();
        }

        static PermissionSet GetPartialTrustPermissions()
        {
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
            return SecurityManager.GetStandardSandbox(evidence);
        }

        static void InitPartialTrustSilo(string[] args)
        {
            string siloName = "PartialTrustSilo";

            var siloHost = new SiloHost(siloName)
            {
                Config = new ClusterConfiguration(dynamicDefaults: false),
                ConfigFileName = args[0],
            };

            siloHost.InitializeOrleansSilo();
        }
    }
}
