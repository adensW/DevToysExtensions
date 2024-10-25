using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Adens.DevToys.RsaGenerator.Helpers;
public static class RsaGeneratorHelper
{
    public static async Task<(string pri,string pub)> GenerateRsaKeyPairAsync(int keySize)
    {
        using var rsa = new RSACryptoServiceProvider(keySize);
        string pri = rsa.ExportRSAPrivateKeyPem();
        string pub = rsa.ExportRSAPublicKeyPem();
        return (pri, pub);
    }
}
