namespace TeamViewer.QuickSupport.Integration
{
    public class AuthInfo
    {
        public AuthInfo(string id, string password)
        {
            ID = id;
            Password = password;
        }

        public string ID { get; private set; }

        public string Password { get; private set; }

        public override string ToString()
        {
            return ID + " : " + Password;
        }
    }
}