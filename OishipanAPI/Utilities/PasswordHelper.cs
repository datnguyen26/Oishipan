namespace OishipanAPI.Utilities
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Some legacy bcrypt implementations use variant prefixes like $2y$ or $2x$
                // Normalize to a supported variant and retry verification.
                try
                {
                    if (hash.Length > 3 && (hash.StartsWith("$2y$") || hash.StartsWith("$2x$")))
                    {
                        var tail = hash.Substring(4);
                        var alt = "$2a$" + tail;
                        if (BCrypt.Net.BCrypt.Verify(password, alt))
                            return true;

                        alt = "$2b$" + tail;
                        if (BCrypt.Net.BCrypt.Verify(password, alt))
                            return true;
                    }
                }
                catch
                {
                    // fall through to return false
                }

                return false;
            }
        }
    }
}
