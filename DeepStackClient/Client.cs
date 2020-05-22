using System;
using System.Threading.Tasks;

namespace DeepStackClient
{
   public class Client
   {
        public static async Task<Client> CreateLoggedInAsync(Uri uri, string username, string password)
        {
            return new Client();
        }

        private Client()
        {

        }
   }
}
