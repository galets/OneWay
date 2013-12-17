using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OneWay
{
    public class Crypto
    {
        static readonly string HEADER = "ASCR";
        const int KEY_SIZE = 4096;
        const int MODULUS_SIZE = KEY_SIZE / 8;


		public static byte[] GetRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using (var urandom = File.OpenRead("/dev/urandom"))
            {
                int l = urandom.Read(bytes, 0, length);
                if (l != length)
                {
                    throw new Exception("could not read random bytes from urandom");
                }
            }

            return bytes;
        }

		public static RSACryptoServiceProvider CreateRsaProvider(string key)
		{
			var alg = new RSACryptoServiceProvider(4096);
			alg.FromXmlString(key);
			return alg;
		}

		public static bool IsValidKey(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return false;
			}

			try
			{
				using (CreateRsaProvider(key)) 
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		public static void Encrypt(Stream input, Stream output, RSACryptoServiceProvider asymmetricAlg)
		{
			var header = Encoding.ASCII.GetBytes(HEADER);
			output.Write(header, 0, header.Length);

			var aes = new RijndaelManaged() { KeySize = 256 };
			aes.GenerateIV();
			output.Write(aes.IV, 0, aes.IV.Length);

			aes.GenerateKey();
			var key = asymmetricAlg.Encrypt(aes.Key, false);
			if (key.Length != MODULUS_SIZE)
			{
				throw new InvalidOperationException();
			}
			output.Write(key, 0, key.Length);

			using (var cs = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
			{
				byte[] buffer = new byte[1024 * 16];
				int count;
				while ((count = input.Read(buffer, 0, buffer.Length)) != 0)
				{
					cs.Write(buffer, 0, count);
				}
			}
		}
    }
}
