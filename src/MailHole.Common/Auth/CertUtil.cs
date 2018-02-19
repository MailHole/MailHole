using System;
using System.IO;
using System.Security.Cryptography;
using MailHole.Common.Model.Auth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MailHole.Common.Auth
{
    public static class CertUtil
    {
        public static SecurityKey LoadECDSACert(string serializedParamsPath)
        {
            if (string.IsNullOrEmpty(serializedParamsPath)) throw new NullReferenceException("Path may not be null or empty");
            var ecDsa = ECDsa.Create();
            if (ecDsa == null) throw new InvalidOperationException();

            if (!File.Exists(serializedParamsPath))
            {
                File.WriteAllText(serializedParamsPath, JsonConvert.SerializeObject(ecDsa.ExportParameters(true).ToEcParamsDto()));
                return new ECDsaSecurityKey(ecDsa);
            }
            
            var ecParams = JsonConvert.DeserializeObject<EcParamsDto>(File.ReadAllText(serializedParamsPath));
            ecDsa.ImportParameters(ecParams.ToEcParameters());
            var key = new ECDsaSecurityKey(ecDsa)
            {
                CryptoProviderFactory = CryptoProviderFactory.Default
            };
            return key;
        }
    }
}