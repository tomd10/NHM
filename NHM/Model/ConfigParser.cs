using NHM.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Model
{
    public static class ConfigParser
    {
        private const string sep = "@@@@";
        public static bool parseConfig(string cfg, out int error)
        {
            error = 0;
            string copy = cfg;
            copy = copy.Replace("\r", "");
            string[] lines = copy.Split("\n");

            //Too short
            if (lines.Length < 2)
            {
                error = -1;
                return false;
            }

            //Wrong format
            if (lines[0] != "Config file for NHM v1.0")
            {
                error = -1;
                return false;
            }

            NetDevice currentDev = null;
            for (int i = 1; i < lines.Length; i++)
            {
                string[] word = lines[i].Split(sep);

                //Empty line?
                if (word.Length < 3)
                {
                    error = i;
                    return false;
                }

                //Condition without device
                if (currentDev == null && word[0] == "CONDITION")
                {
                    error = -2;
                    return false;
                }
                if (word[0] == "DEVICE")
                {
                    //Wrong fmt
                    if (word.Length != 7)
                    {
                        error = i;
                        return false;
                    }

                    string name = word[1];
                    string hostname = word[2];
                    int port = 22; int.TryParse(word[3], out port);
                    string username = word[4];
                    string password = word[5];
                    string[] initCommands = word[6].Split("####");

                    NetDevice d = new NetDevice(name, hostname, port, username, password, initCommands);
                    currentDev = d;
                    DataStore.devices.Add(d);
                }
                else if (word[0] == "CONDITION")
                {
                    int mode = 0;
                    if (!int.TryParse(word[2], out mode)) //Mode int parse err
                    {
                        error = i;
                        return false;
                    }

                    if (mode == 0)
                    {
                        if (word.Length != 4) //Length err
                        {
                            error = i;
                            return false;
                        }
                        string name = word[1];
                        string[] commands = word[3].Split("####");

                        DeviceCondition dc = DeviceCondition.DevC0(name, commands);
                        currentDev.AddCondition(dc);
                    }
                    else if (mode == 1)
                    {
                        if (word.Length != 11) //Length err
                        {
                            error = i;
                            return false;
                        }
                        bool flag = true;
                        string name = word[1];
                        string command = word[3];
                        int start = 0, end = 0, line = 0;
                        flag = flag && int.TryParse(word[4], out start);
                        flag = flag && int.TryParse(word[5], out end);
                        flag = flag && int.TryParse(word[6], out line);
                        string[] preCommands = word[7].Split("####");
                        string[] postCommands = word[8].Split("####");
                        string separator = word[9] == "" ? " " : word[9];
                        bool trimSeparator = true;
                        flag = flag && bool.TryParse(word[10], out trimSeparator);

                        if (!flag && error == 0) error = i;

                        DeviceCondition dc = DeviceCondition.DevC1(name, command, start, end, line, preCommands, postCommands, separator, trimSeparator);
                        currentDev.AddCondition(dc);

                    }
                    else if (mode == 2)
                    {
                        if (word.Length != 8) //Length err
                        {
                            error = i;
                            return false;
                        }

                        bool flag = true;

                        string name = word[1];
                        string command = word[3];
                        int start = 0, end = 0;
                        flag = flag && int.TryParse(word[4], out start);
                        flag = flag && int.TryParse(word[5], out end);
                        string[] preCommands = word[6].Split("####");
                        string[] postCommands = word[7].Split("####");

                        if (!flag && error == 0) error = i;

                        DeviceCondition dc = DeviceCondition.DevC2(name, command, start, end, preCommands, postCommands);
                        currentDev.AddCondition(dc);
                    }
                    else if (mode == 3 || mode == 4 || mode == 5 || mode == 6)
                    {
                        if (word.Length != 12) //Length err
                        {
                            error = i;
                            return false;
                        }

                        bool flag = true;

                        string name = word[1];
                        string command = word[3];
                        int start = 0, line = 0, thr = 0;
                        flag = flag && int.TryParse(word[4], out start);
                        flag = flag && int.TryParse(word[5], out line);
                        flag = flag && int.TryParse(word[6], out thr);
                        string[] preCommands = word[7].Split("####");
                        string[] postCommands = word[8].Split("####");
                        string separator = word[9] == "" ? " " : word[9];
                        bool trimSeparator = true, trimNonNum = true;
                        flag = flag && bool.TryParse(word[10], out trimSeparator);
                        flag = flag && bool.TryParse(word[11], out trimNonNum);

                        if (!flag && error == 0) error = i;

                        DeviceCondition dc = DeviceCondition.DevC3(name, mode, command, start, line, thr, preCommands, postCommands, separator, trimSeparator, trimNonNum);
                        currentDev.AddCondition(dc);

                    }
                    else if (mode == 7 || mode == 8 || mode == 9 || mode == 10)
                    {
                        if (word.Length != 7) //Length err
                        {
                            error = i;
                            return false;
                        }

                        bool flag = true;
                        string name = word[1];
                        string command = word[3];
                        int thr = 0;
                        flag = flag && int.TryParse(word[4], out thr);
                        string[] preCommands = word[5].Split("####");
                        string[] postCommands = word[6].Split("####");

                        if (!flag && error == 0) error = i;

                        DeviceCondition dc = DeviceCondition.DevC7(name, mode, command, thr, preCommands, postCommands);
                        currentDev.AddCondition(dc);
                    }
                    else if (mode == 11 || mode == 12)
                    {
                        if (word.Length != 12) //Length err
                        {
                            error = i;
                            return false;
                        }

                        bool flag = true;
                        string name = word[1];
                        string command = word[3];
                        int start = 0, end = 0, line = 0;
                        flag = flag && int.TryParse(word[4], out start);
                        flag = flag && int.TryParse(word[5], out end);
                        flag = flag && int.TryParse(word[6], out line);
                        string sub = word[7];
                        string[] preCommands = word[8].Split("####");
                        string[] postCommands = word[9].Split("####");
                        string separator = word[10] == "" ? " " : word[10];
                        bool trimSeparator = true;
                        flag = flag && bool.TryParse(word[11], out trimSeparator);

                        if (!flag && error == 0) error = i;
                        DeviceCondition dc = DeviceCondition.DevC11(name, mode, command, start, end, line, sub, preCommands, postCommands, separator, trimSeparator);
                        currentDev.AddCondition(dc);
                    }
                    else if (mode == 13 || mode == 14)
                    {
                        if (word.Length != 9) //Length err
                        {
                            error = i;
                            return false;
                        }

                        bool flag = true;
                        string name = word[1];
                        string command = word[3];
                        int start = 0, end = 0;
                        flag = flag && int.TryParse(word[4], out start);
                        flag = flag && int.TryParse(word[5], out end);
                        string sub = word[6];
                        string[] preCommands = word[7].Split("####");
                        string[] postCommands = word[8].Split("####");

                        if (!flag && error == 0) error = i;
                        DeviceCondition dc = DeviceCondition.DevC13(name, mode, command, start, end, sub, preCommands, postCommands);
                        currentDev.AddCondition(dc);
                    }
                    else //Unknown line
                    {
                        error = i;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
