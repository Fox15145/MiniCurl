using System.Net.Http;

namespace MiniCurl
{
    class Program
    {    
        static HttpClient client = new HttpClient();
        static HttpClient smsClient = new HttpClient();
        static HttpResponseMessage theMsg;
        
        const string smsErrorLink = "https://bit.ly/WebServer_Ok";
        const string smsGoodLink = "https://bit.ly//WebServer_Ko";
        
        static string filename;
        static string myUri;
        
        static void Main(string[] args)
        {
            // Site à tester passé en argument
            if (args.Length > 0)
            {
                myUri = args[0];
                client.BaseAddress = new Uri(myUri);

                // Nom du fichier "Flag"
                filename = $"{myUri.Replace("http://", "").Replace("/", "_")}.down";
                try
                {
                    HttpResponseMessage myresponseMsg = client.GetAsync(client.BaseAddress).Result;

                    //Console.WriteLine(myresponseMsg);
                    //Console.ReadLine();
                    if ((System.IO.File.Exists(filename)) && (myresponseMsg.IsSuccessStatusCode))
                    {
                        SendLinkIsUp(myresponseMsg.ToString());
                    }
                    else
                    if (!myresponseMsg.IsSuccessStatusCode)
                    {
                        SendLinkIsDown(myresponseMsg.ToString());
                    }
                }
                catch (Exception e)
                {
                    SendError(e.Message);
                }

            }
        }
        
        private static void SendLinkIsUp(string message)
        {
            if (System.IO.File.Exists(filename))
            {
                string msg = $"Hôte {myUri} de nouveau accessible.{Environment.NewLine}{message}";

                // suppresion du fichier "Flag"
                System.IO.File.Delete(filename);

                // envoi par webApi      
                theMsg = client.GetAsync(smsGoodLink).Result;

                // envoi par mail                        
                Mail.Send("Good@link.com", "me@mail.com", myUri, msg, false);           
            }
        }
            
        private static void SendLinkIsDown(string message)
        {
            if (!System.IO.File.Exists(filename))
            {
                string msg = $"Hôte {myUri} inaccessible.{Environment.NewLine}{message}";

                // Ecriture du "Flag"
                System.IO.File.WriteAllText(filename, msg);

                // envoi par webApi
                theMsg = client.GetAsync(smsErrorLink).Result;

                // envoi par mail                                        
                Mail.Send("Good@link.com", "me@mail.com", myUri, msg, false);               
            }
        }
    }
}
