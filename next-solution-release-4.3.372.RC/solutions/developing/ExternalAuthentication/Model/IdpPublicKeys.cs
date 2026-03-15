



namespace ExternalAuthentication.Model
{
    // Public parameters use a lower-case first letter for correct JSON deserialization
    public class IdpPublicKeys
    {
        public IdpPublicKey[] keys { get; set; }
    }

    public class IdpPublicKey
    {
        public string kty { get; set; }
        public string use { get; set; }
        public string kid { get; set; }
        public string x5t { get; set; }
        public string n { get; set; }
        public string e { get; set; }
        public string[] x5c { get; set; }
    }
}
