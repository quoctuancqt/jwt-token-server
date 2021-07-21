namespace JwtTokenServer
{
    public class JwtSettings
    {
        public const string DefaultSecretKey = "9@Hyn7!?_nncJ6w3%v4Avh7y=^^WC&dL";

        public const string DefaultPath = "/token";
    }

    public class JwtOptions
    {
        public static string Name = "JwtOptions";
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
