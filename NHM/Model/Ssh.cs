using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Model
{
    public static class SshConnection
    {
        private static SshClient client;
        public static string GetResponse(string command)
        {
            
            return "";
        }

        public static bool? OpenConnection(string hostname, string username, string password)
        {
            client = new SshClient(hostname, username, password);
            client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(5);
            try
            {
                client.Connect();
            }
            catch (Renci.SshNet.Common.SshAuthenticationException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return false;
            }
            /*
            catch (Exception e)
            {
                return false;
            }*/
            
            var x = client.ConnectionInfo;
            return true;
            
        }

        public static void CloseConnection()
        {
            client.Disconnect();
        }
    }
}
