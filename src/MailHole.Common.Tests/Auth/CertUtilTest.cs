using System;
using System.Security.Cryptography;
using MailHole.Common.Model.Auth;
using Newtonsoft.Json;
using Xunit;

namespace MailHole.Common.Tests.Auth
{
    public class CertUtilTest
    {
        [Fact]
        public void TestExportEcdSa()
        {
            var ecDsa = ECDsa.Create();
            var ecParameters = ecDsa.ExportParameters(true);


            var serializedString = JsonConvert.SerializeObject(ecParameters.ToEcParamsDto());
            var ecParams2 = JsonConvert.DeserializeObject<EcParamsDto>(serializedString).ToEcParameters();

            var ecDsa2 = ECDsa.Create();
            ecDsa2.ImportParameters(ecParams2);

            Console.Out.WriteLine("complete");
        }
    }
}