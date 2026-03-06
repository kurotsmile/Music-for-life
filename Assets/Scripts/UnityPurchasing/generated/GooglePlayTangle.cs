// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("KJKHauTGvpPegiHKweDa5esn3EQ85n/JY3B2dxVZlWAiDsp8oGXxwVurGTtTYHZELP4yWtrtOna7V674O9MuSAvQbZbi9qGtyrDq1jT/SwIXzZhEiSaAouOpwV9sEFMJfdItHfl6dHtL+Xpxefl6enuisZBBifj1uoXUQWoBoxtD4tQbsVKbILVO5xhL+XpZS3Z9clH9M/2Mdnp6en57eKMTyU53l97GyFG1MnBbnvPH7dgoHzWVxbWjONzTT1yaVShrps3isWTDIc0jVj5LS+bNMi0F9ixwPBtEUI0U5EdgjJn1U+In7EzXE70fYfnhMmrHC0TvhPjsSh7gUvxGZYG1XAynv+k2S1q+GeDbtLOZLNs1m0CCb2NBUAGeZDVAonl4ent6");
        private static int[] order = new int[] { 13,7,11,9,5,7,11,13,11,13,13,12,12,13,14 };
        private static int key = 123;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
