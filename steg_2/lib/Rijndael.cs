using steg_2.lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace steg_2
{
    public class Rijndael
    {
    public static byte[] Encrypt(byte[] data, byte[] key)
        {

            byte[] keyExtension = new byte[32] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                                                 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
            for(int i =0; i< (key.Length % keyExtension.Length); i++)
            {
                keyExtension[i] = key[i];
            }
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                csp.KeySize = 256;
                csp.BlockSize = 128;
                csp.Key = keyExtension;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform encrypter = csp.CreateEncryptor();
                return encrypter.TransformFinalBlock(data, 0, data.Length);
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            byte[] keyExtension = new byte[32] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                                                 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
            for (int i = 0; i < (key.Length % keyExtension.Length); i++)
            {
                keyExtension[i] = key[i];
            }
            using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
            {
                csp.KeySize = 256;
                csp.BlockSize = 128;
                csp.Key = keyExtension;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform decrypter = csp.CreateDecryptor();
                return decrypter.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }
}
