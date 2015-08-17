namespace Orleans.Runtime
{
    using System;
    using System.Security;
    using System.Security.Policy;

    using TaskRemoting;

    /// <summary>
    /// Provides sandbox, used to host grain instances
    /// </summary>
    sealed class Sandbox
    {
        readonly AppDomain domain;

        // TODO: must be configurable
        public Sandbox()
        {
            var permissions = this.GetPartialTrustPermissions();
            var domainSetup = new AppDomainSetup
            {
                // TODO: SECURITY: must be provided from outside
                ApplicationBase = Environment.CurrentDirectory,
            };
            var trustedAssemblies = new StrongName[] {
                typeof(GrainReference).Assembly.GetStrongName(),
                typeof(RemoteTask).Assembly.GetStrongName(),
            };

            this.domain = AppDomain.CreateDomain("Grain Sandbox", null, domainSetup, permissions, trustedAssemblies);
        }

        public MarshalByRefObject CreateInstance(Type type)
        {
            var instance = this.domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            return (MarshalByRefObject)instance;
        }

        PermissionSet GetPartialTrustPermissions()
        {
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
            return SecurityManager.GetStandardSandbox(evidence);
        }

        public AppDomain Domain { get { return this.domain; } }
    }
}
