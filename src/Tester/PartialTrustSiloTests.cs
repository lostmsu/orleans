namespace Tester
{
    using System;
    using System.Security;
    using System.Security.Policy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartialTrustSiloTests
    {
        [TestMethod]
        public void CanStartSiloHostInPartialTrust()
        {
            var permissions = GetPartialTrustPermissions();
            var applicationBase = System.IO.Path.GetDirectoryName(typeof(PartialTrustSiloTests).Assembly.Location);
            var hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitPartialTrustSilo,
                ApplicationBase = applicationBase,
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

        }
    }
}
