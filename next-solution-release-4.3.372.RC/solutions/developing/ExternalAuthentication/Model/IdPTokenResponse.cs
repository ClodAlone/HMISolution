




namespace ExternalAuthentication.Model
{
    // Public parameters use a lower-case first letter for correct JSON deserialization
    public class IdPTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string scope { get; set; }
        public string id_token { get; set; }
    }
}
