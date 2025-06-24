namespace WebAPIDemo.Authority
{
    public static class AppRepository
    {
        private static List<Application> _applications = new List<Application>()
        {
            new Application
            {
                ApplicationId = 1,
                ApplicationName = "MVCWebApp",
                ClientId="1233",
                Secret="123",
                Scopes="read,write"
            }
        };

        
        public static Application? GetApplicationByClientId(string clientid) 
        { 
            return _applications.FirstOrDefault(x=>x.ClientId == clientid);
        }
    }
}
