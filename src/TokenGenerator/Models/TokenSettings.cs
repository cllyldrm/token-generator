namespace TokenGenerator.Models
{
    public class TokenSettings
    {
        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string SigningKey { get; set; }

        public int ExpireMinute { get; set; }
    }
}