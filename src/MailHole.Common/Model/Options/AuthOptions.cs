using System;
using System.Threading;
using MailHole.Common.Auth;
using Microsoft.IdentityModel.Tokens;

namespace MailHole.Common.Model.Options
{
    public class AuthOptions
    {
        private readonly Lazy<SecurityKey> _securityKeyLazy;
        private readonly Lazy<SigningCredentials> _signingCredentialsLazy;

        public AuthOptions()
        {
            _securityKeyLazy = new Lazy<SecurityKey>(() => CertUtil.LoadECDSACert(SigningCertPath), LazyThreadSafetyMode.None);
            _signingCredentialsLazy = new Lazy<SigningCredentials>(() => new SigningCredentials(SecurityKey, SecurityAlgorithms.EcdsaSha512));
        }


        public string Issuer { get; set; } = "MailHole.API";

        public string Audience { get; set; } = "MailHole";

        public string SigningCertPath { get; set; } = "/app/signingCert.json";

        public SigningCredentials SigningCredentials => _signingCredentialsLazy.Value;

        public SecurityKey SecurityKey => _securityKeyLazy.Value;
    }
}