namespace UdemyCore.Configuration
{
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> Audiences { get; set; } // hangi api'lere erişebileceğini belirteceğiz
    }
}
