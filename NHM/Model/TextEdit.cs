using NHM.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Model
{
    public static class TextEdit
    {
        public static string[] Split(string toSplit, string sep, bool trimSeparator)
        {
            string splitting = toSplit;
            if (trimSeparator)
            {
                while (splitting.Contains(sep + sep))
                {
                    splitting = splitting.Replace(sep + sep, sep);
                }
            }

            return splitting.Split(sep);
        }

        public static string[] SplitLines(string toSplit)
        {
            if (toSplit.Contains('\r') && !toSplit.Contains('\n')) return toSplit.Split('\r');
            return toSplit.Replace("\r", "").Split('\n');
        }

        public static string GetLine(string s, int line, out bool error)
        {
            string[] lines = SplitLines(s);
            if (lines.Length <= line)
            {
                error = true;
                return null;
            }
            error = false;
            return lines[line];
        }

        public static string WordRange(string s, int line, int start, int end, string sep, bool trimSeparator, out bool error)
        {
            bool err = false;
            string l = GetLine(s, line, out err);

            if (err) { error = true; return null; }

            string[] words = Split(l, sep, trimSeparator);

            if (start >= words.Length || end >= words.Length || start > end) { error = true; return null; }
            
            string[] result = new string[end-start+1];
            Array.Copy(words, start, result, 0, end - start + 1);
            error = false;
            return String.Join(" ", result);
        }

        public static string LineRange(string s, int start, int end, out bool error)
        {
            string[] lines = SplitLines(s);

            if (start >= lines.Length || end >= lines.Length || start > end) { error = true; return null; }

            string[] result = new string[end - start + 1];
            Array.Copy(lines, start, result, 0, end - start + 1);
            error = false;
            return String.Join('\n', result);

        }

        public static string LineRangeMax(string s, int start, int end)
        {
            string[] lines = SplitLines(s);
            if (start >= lines.Length) return "";
            
            int end_ = end >= lines.Length ? lines.Length-1 : end;

            string[] result = new string[end_ - start + 1];
            Array.Copy(lines, start, result, 0, end_ - start + 1);
            return String.Join('\n', result);

        }

        public static int Int(string s, bool trimNonNum, out bool error)
        {
            string num = "";
            if (trimNonNum)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if ((int)s[i] >= 48 && (int)s[i] <= 57) num = num + s[i];
                }
            }
            else num = s;

            int result;
            bool err = !int.TryParse(num, out result);

            error = err;
            return result;
        }

        public static int LineCount(string s)
        {
            if (s == "" || s == null) return 0;
            return SplitLines(s).Length;
        }

        public static string Serialize()
        {
            string output = "";
            output = output + "UTC " + DateTime.UtcNow.ToString();
            output = output + "\nLocal " + DateTime.Now.ToString();
            output = output + "\nUnreachable devices: ";
            foreach (NetDevice d in DataStore.unreachableDevices) output = output + d.getName() + "[" + d.GetHostname() + "] ";
            output = output + "\nAuth failed devices: ";
            foreach (NetDevice d in DataStore.authDevices) output = output + d.getName() + "[" + d.GetHostname() + "] ";
            output = output + "\nAvailable devices: ";
            foreach (NetDevice d in DataStore.availableDevices) output = output + d.getName() + "[" + d.GetHostname() + "] ";
            output = output + "\nProcessed devices: ";
            foreach (NetDevice d in DataStore.processedDevices) output = output + d.getName() + "[" + d.GetHostname() + "] ";
            output = output + "\n";
            foreach (NetDevice d in DataStore.processedDevices)
            {
                output = output + d.getName() + "\n";
                foreach (DeviceCondition dc in d.list)
                {
                    output = output + dc.cp.name + "\n";
                    output = output + dc.cp.lines + "\n";
                }
                output = output + "\n";
            }
            return output;
        }

    }
}
