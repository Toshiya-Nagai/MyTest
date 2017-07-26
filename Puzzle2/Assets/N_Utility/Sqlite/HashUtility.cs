public static class HashUtility {
	public static string MD5(string path) {
		using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			return retVal.ToHex();
		}
	}
	
	public static string ToHex(this byte[] bytes) {
		char[] c = new char[bytes.Length * 2];
		
		byte b;
		
		for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx) {
			b = ((byte)(bytes[bx] >> 4));
			c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
			
			b = ((byte)(bytes[bx] & 0x0F));
			c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
		}
		
		return new string(c);
	}
}