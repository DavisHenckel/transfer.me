﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace Client.Crypto
{
    public class AES
    {
        const string ENCRYPTION_MODE = "AES/CBC/PKCS7Padding";

        public static byte[] Encrypt(byte[] plaintText, ParametersWithIV keyParams)
        {
            //Cipher chosen based on desired algorithm
            IBufferedCipher cipher = CipherUtilities.GetCipher(ENCRYPTION_MODE);
            cipher.Init(true, keyParams);
            //plainText encrypted
            byte[] cipherText = cipher.DoFinal(plaintText);
            byte[] iv = keyParams.GetIV();
            //return cipherText;
            return iv.Concat(cipherText).ToArray();
        }

        //Decrypt method accepts cipher text and key
        //unpacks ciphertext to get IV and encrypted data
        //decrypts ciphertext and returns plaintext
        public static byte[] Decrypt(byte[] cipherText, string key)
        {
            byte[] symmetricKey = Convert.FromBase64String(key);
            KeyParameter kp = new KeyParameter(symmetricKey);
            byte[] iv = new byte[16];
            byte[] UnpackedCipherText = new byte[cipherText.Length - 16];
            Array.Copy(cipherText, 0, iv, 0, 16);
            Array.Copy(cipherText, 16, UnpackedCipherText, 0, cipherText.Length - 16);
            ParametersWithIV keyParameters = new ParametersWithIV(kp, iv);
            //Initialize cipher in decryption mode by entering false in the mode field
            IBufferedCipher cipher = CipherUtilities.GetCipher(ENCRYPTION_MODE);
            cipher.Init(false, keyParameters);
            byte[] decrypted = cipher.DoFinal(UnpackedCipherText);
            return decrypted;
        }

        //CipherUtilities in Security. Check out IBufferedCipher
        //create  cipher

        public static ParametersWithIV GenerateKeyWithIV(byte[] symmetricKey)
        {
            SecureRandom random = new SecureRandom();
            byte[] iv = random.GenerateSeed(16);
            //Creates KeyGenerator for AES with 128 bit key size
            //CipherKeyGenerator generator = GeneratorUtilities.GetKeyGenerator("AES128");
            //secret symmetricKey generated
            //byte[] symmetricKey = generator.GenerateKey();
            KeyParameter keyParam = new KeyParameter(symmetricKey);
            return new ParametersWithIV(keyParam, iv);
        }

        public static byte[] KeyGen()
        {
            CipherKeyGenerator generator = GeneratorUtilities.GetKeyGenerator("AES128");
            //secret symmetricKey generated
            return generator.GenerateKey();
        }
    }
}
