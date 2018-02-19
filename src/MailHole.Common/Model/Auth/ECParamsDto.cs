using System.Security.Cryptography;

namespace MailHole.Common.Model.Auth
{
    public class EcParamsDto
    {
        public byte[] D { get; set; }

        public byte[] X { get; set; }

        public byte[] Y { get; set; }

        public string CurveName { get; set; }

        public ECParameters ToEcParameters()
        {
            return new ECParameters
            {
                D = D,
                Q = new ECPoint
                {
                    X = X,
                    Y = Y
                },
                Curve = ECCurve.CreateFromFriendlyName(CurveName)
            };
        }
    }

    public static class EcParamsDtoExtensions
    {
        public static EcParamsDto ToEcParamsDto(this ECParameters ecParameters)
        {
            return new EcParamsDto
            {
                D = ecParameters.D,
                X = ecParameters.Q.X,
                Y = ecParameters.Q.Y,
                CurveName = ecParameters.Curve.Oid.FriendlyName
            };
        }
    }
}