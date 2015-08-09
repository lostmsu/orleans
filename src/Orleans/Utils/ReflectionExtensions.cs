namespace Orleans
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Security.Policy;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets strong name of the provided assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static StrongName GetStrongName(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var assemblyInfo = assembly.GetName();
            var publicKey = assemblyInfo.GetPublicKey();
            if (publicKey == null)
                throw new InvalidOperationException("Assembly is not strong-named");

            var publicKeyBlob = new StrongNamePublicKeyBlob(publicKey);
            return new StrongName(publicKeyBlob, assemblyInfo.Name, assemblyInfo.Version);
        }
    }
}
