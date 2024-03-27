using NHM.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Model
{
    public static class DataStore
    {
        public static bool configured = false;
        public static string configuration = "";

        public static bool decrypted = false;
        public static string decryptedConfiguration = "";

        public static bool parsed = false;

        public static bool processed = false;

        public static List<NetDevice> devices = new List<NetDevice>();
        public static List<NetDevice> availableDevices = new List<NetDevice>();
        public static List<NetDevice> unreachableDevices = new List<NetDevice>();
        public static List<NetDevice> authDevices = new List<NetDevice>();
        public static List<NetDevice> processedDevices = new List<NetDevice>();

    }
}
