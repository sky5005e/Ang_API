using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BSFinancial.Providers
{
	public class GuidEncryptionProvider
	{
		private static readonly GuidEncryptionProvider instance;
		private readonly string PasswordHash = "Pa@Sw0RD";
		private readonly string SaltKey = "s@Lt+KeY";
		private readonly string VIKey = "z1%7X3_8f5#4g7@6";

		public static GuidEncryptionProvider Instance
		{
			get { return instance; }
		}

		static GuidEncryptionProvider()
		{
			instance = new GuidEncryptionProvider();
		}

		public GuidEncryptionProvider() {}

		public GuidEncryptionProvider(string password, string salt, string vi)
		{
			PasswordHash = password;
			SaltKey = salt;
			VIKey = vi;
		}

		#region Private

		private string UrlTokenEncode(byte[] input)
		{
			if (input == null) {
				throw new ArgumentNullException("input");
			}
			if (input.Length < 1) {
				return string.Empty;
			}
			string str = Convert.ToBase64String(input);
			if (str == null) {
				return (string) null;
			}
			int length = str.Length;
			while (length > 0 && (int) str[length - 1] == 61) {
				--length;
			}
			var chArray = new char[length + 1];
			chArray[length] = (char) (48 + str.Length - length);
			for (int index = 0; index < length; ++index) {
				char ch = str[index];
				switch (ch) {
					case '+':
						chArray[index] = '-';
						break;
					case '/':
						chArray[index] = '_';
						break;
					case '=':
						chArray[index] = ch;
						break;
					default:
						chArray[index] = ch;
						break;
				}
			}
			return new string(chArray);
		}

		private byte[] UrlTokenDecode(string input)
		{
			if (input == null) {
				throw new ArgumentNullException("input");
			}
			int length = input.Length;
			if (length < 1) {
				return new byte[0];
			}
			int num = (int) input[length - 1] - 48;
			if (num < 0 || num > 10) {
				return (byte[]) null;
			}
			var inArray = new char[length - 1 + num];
			for (int index = 0; index < length - 1; ++index) {
				char ch = input[index];
				switch (ch) {
					case '-':
						inArray[index] = '+';
						break;
					case '_':
						inArray[index] = '/';
						break;
					default:
						inArray[index] = ch;
						break;
				}
			}
			for (int index = length - 1; index < inArray.Length; ++index) {
				inArray[index] = '=';
			}
			return Convert.FromBase64CharArray(inArray, 0, inArray.Length);
		}

		private string Encode(Guid guid)
		{
			string enc = Convert.ToBase64String(guid.ToByteArray());
			enc = enc.Replace("/", "_");
			enc = enc.Replace("+", "-");
			return enc.Substring(0, 22);
		}

		private Guid Decode(string encoded)
		{
			encoded = encoded.Replace("_", "/");
			encoded = encoded.Replace("-", "+");
			var buffer = Convert.FromBase64String(encoded + "==");
			return new Guid(buffer);
		}

		private byte[] Encrypt(byte[] plainTextBytes)
		{
			//byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

			var keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256/8);
			var symmetricKey = new RijndaelManaged() {Mode = CipherMode.CBC, Padding = PaddingMode.Zeros};
			var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

			byte[] cipherTextBytes;

			using (var memoryStream = new MemoryStream()) {
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
				}
			}
			//return Convert.ToBase64String(cipherTextBytes);
			return cipherTextBytes;
		}

		private byte[] Decrypt(byte[] cipherTextBytes)
		{
			//byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
			var keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256/8);
			var symmetricKey = new RijndaelManaged() {Mode = CipherMode.CBC, Padding = PaddingMode.None};

			byte[] plainTextBytes;
			var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			using (var memoryStream = new MemoryStream(cipherTextBytes)) {
				using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
					plainTextBytes = new byte[cipherTextBytes.Length];
					int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
				}
			}
			//return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
			return plainTextBytes;
		}

		#endregion

		public string GuidToEncUrlBase64(Guid guid)
		{
			var guidBytes = guid.ToByteArray();
			//var encryptedBytes = Encrypt(guidBytes);
			//string base64 = UrlTokenEncode(encryptedBytes);
            string base64 = UrlTokenEncode(guidBytes);
			return base64;
		}

		public Guid? EncUrlBase64ToGuid(string base64)
		{
			try {
				var encryptedBytes = UrlTokenDecode(base64);
				//var guidBytes = Decrypt(encryptedBytes);
				//var guid = new Guid(guidBytes);
                var guid = new Guid(encryptedBytes);
				return guid;
			}
			catch (Exception e) {
				return null;
			}
		}

	}
}