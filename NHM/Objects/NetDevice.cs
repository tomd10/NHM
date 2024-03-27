using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Objects
{
    public class NetDevice
    {
        private string name;
        private string hostname;
        private int port;
        private string username;
        private string password;
        private string[] initCommands;

        private SshClient client;

        public List<DeviceCondition> list;

        public NetDevice(string name, string hostname, int port, string username, string password, string[] initCommands)
        {
            this.name = name;
            this.hostname = hostname;
            this.port = port;
            this.username = username;
            this.password = password;
            this.initCommands = initCommands;

            list = new List<DeviceCondition>();
        }

        public void AddCondition(DeviceCondition dc)
        {
            list.Add(dc);
        }

        public bool? TestSsh()
        {
            client = new SshClient(hostname, port, username, password);
            client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(4);
           
            
            try
            {
                client.Connect();

                foreach (string s in initCommands)
                {
                    if (s != "" && s != null)
                    {
                        if (!client.IsConnected) client.Connect();
                        client.RunCommand(s);
                    }
                    
                }
            }
            catch (Renci.SshNet.Common.SshAuthenticationException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return false;
            }

            client.Disconnect();
            return true;
        }

        public string getName()
        {
            return this.name;
        }

        public string GetHostname()
        {
            return this.hostname;
        }

        public bool GetReplies()
        {
            try
            {
                client = new SshClient(hostname, port, username, password);
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(4);
                client.KeepAliveInterval = TimeSpan.FromSeconds(2);
                client.Connect();
                foreach (DeviceCondition dc in list)
                {
                    string[] pre = dc.GetPreCommands();
                    string[] post = dc.GetPostCommands();
                    string cmd = dc.GetCommand();

                    foreach (string s in pre)
                    {
                        if (s != "" && s != null)
                        {
                            if (!client.IsConnected) client.Connect();
                            client.RunCommand(s);
                        }   
                    }
                    if (!client.IsConnected) client.Connect();
                    SshCommand reply = client.RunCommand(cmd);
                    dc.SetReply(reply.Result);

                    foreach (string s in post)
                    {
                        if (s != "" && s != null)
                        {
                            if (!client.IsConnected) client.Connect();
                            client.RunCommand(s);
                        }
                    }

                    dc.Evaluate();
                    Task.Delay(50);
                }
                
            }
            catch
            {
                return false;
            }
            finally
            {
                if (!client.IsConnected) client.Disconnect();
                client.Dispose();

            }
            return true;
        }
    }
}
