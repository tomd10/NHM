using Microsoft.Maui.Graphics;
using NHM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHM.Objects
{
    public class DeviceCondition
    {
        private string name;
        private int mode;

        private string command;
        private int start;
        private int end;
        private int line;

        private int thr;
        private string sub;

        private string[] preCommands;
        private string[] postCommands;
        private string separator;
        private bool trimSeparator;
        private bool trimNonNum;

        private string reply;
        private string trimmedReply;

        public ConditionPresentation cp;
        private bool failed;
        private int errorReason = 0; //1 - out of range, 2 - int conversion error, 3 - arithmetic error


        //Mode 0
        private DeviceCondition(string name, int mode, string[] commands)
        {
            this.name = name;
            this.mode = mode;
            this.preCommands = commands;
        }

        //Mode 1
        private DeviceCondition(string name, int mode, string command, int start, int end, int line, string[] preCommands, string[] postCommands, string separator, bool trimSeparator)
        {
            this.name = name;
            this.mode = mode;
            this.command = command;
            this.start = start;
            this.end = end;
            this.line = line;
            this.preCommands = preCommands;
            this.postCommands = postCommands;
            this.separator = separator;
            this.trimSeparator = trimSeparator;
        }

        //Mode 2
        private DeviceCondition(string name, int mode, string command, int start, int end, string[] preCommands, string[] postCommands)
        {
            this.name = name;
            this.mode = mode;
            this.command = command;
            this.start = start;
            this.end = end;
            this.preCommands = preCommands;
            this.postCommands = postCommands;

        }

        //Mode 3, 4, 5, 6
        private DeviceCondition(string name, int mode, string command, int start, int line, int thr, string[] preCommands, string[] postCommands, string separator, bool trimSeparator, bool trimNonNum)
        {
            this.name = name;
            this.mode = mode;
            this.command = command;
            this.start = start;
            this.line = line;
            this.thr = thr;
            this.preCommands = preCommands;
            this.postCommands = postCommands;
            this.separator = separator;
            this.trimSeparator = trimSeparator;
            this.trimNonNum = trimNonNum;

        }

        //Mode 7, 8, 9, 10
        private DeviceCondition(string name, int mode, string command, int thr, string[] preCommands, string[] postCommands)
        {
            this.name = name;
            this.mode = mode;
            this.command = command;
            this.thr = thr;
            this.preCommands = preCommands;
            this.postCommands = postCommands;
        }

        //Mode 11, 12
        private DeviceCondition(string name, int mode, string command, int start, int end, int line, string sub, string[] preCommands, string[] postCommands, string separator, bool trimSeparator)
        {
            this.name = name;
            this.mode = mode;
            this.command = command;
            this.start = start;
            this.end = end;
            this.line = line;
            this.sub = sub;
            this.preCommands = preCommands;
            this.postCommands = postCommands;
            this.separator = separator;
            this.trimSeparator = trimSeparator;
        }

        //Mode 13, 14
        private DeviceCondition(string name, int mode, string command, int start, int end, string sub, string[] preCommands, string[] postCommands)
        {
            this.name = name;
            this.mode = mode;
            this.command = command;
            this.start = start;
            this.end = end;
            this.sub = sub;
            this.preCommands = preCommands;
            this.postCommands = postCommands;
        }

        public static DeviceCondition DevC0 (string name, string[] commands)
        {
            return new DeviceCondition(name, 0 ,commands);
        }

        public static DeviceCondition DevC1 (string name, string command, int start, int end, int line, string[] preCommands, string[] postCommands, string separator, bool trimSeparator)
        {
            return new DeviceCondition(name, 1, command, start, end, line, preCommands,postCommands, separator, trimSeparator);
        }

        public static DeviceCondition DevC2 (string name, string command, int start, int end, string[] preCommands, string[] postCommands)
        {
            return new DeviceCondition(name, 2, command, start, end, preCommands, postCommands);
        }

        public static DeviceCondition DevC3 (string name, int mode, string command, int start, int line, int thr, string[] preCommands, string[] postCommands, string separator, bool trimSeparator, bool trimNonNum)
        {
            return new DeviceCondition(name, mode, command, start, line, thr, preCommands, postCommands, separator, trimSeparator, trimNonNum);
        }

        public static DeviceCondition DevC7 (string name, int mode, string command, int thr, string[] preCommands, string[] postCommands)
        {
            return new DeviceCondition(name, mode, command, thr, preCommands, postCommands);
        }

        public static DeviceCondition DevC11 (string name, int mode, string command, int start, int end, int line, string sub, string[] preCommands, string[] postCommands, string separator, bool trimSeparator)
        {
            return new DeviceCondition(name, mode, command, start, end, line, sub, preCommands, postCommands, separator, trimSeparator);
        }

        public static DeviceCondition DevC13 (string name, int mode, string command, int start, int end, string sub, string[] preCommands, string[] postCommands)
        {
            return new DeviceCondition(name, mode, command, start, end, sub, preCommands, postCommands);
        }


        public string[] GetPreCommands()
        {
            return this.preCommands;
        }

        public string[] GetPostCommands()
        {
            return this.postCommands;
        }

        public string GetCommand()
        {
            return this.command;
        }

        public void SetReply(string reply)
        {
            this.reply = reply;
        }

        public void Evaluate()
        {
            switch (this.mode)
            {
                case 0:
                    failed = false;
                    trimmedReply = String.Join('\n', preCommands);
                    break;
                case 1:
                    trimmedReply = TextEdit.WordRange(reply, line, start, end, separator, trimSeparator, out failed);
                    if (failed) errorReason = 1; 
                    break;
                case 2:
                    trimmedReply = TextEdit.LineRangeMax(reply, start, end);
                    if (failed) errorReason = 1;
                    break;
                case 3:
                    trimmedReply = TextEdit.WordRange(reply, line, start, start, separator, trimSeparator, out failed);
                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        int res = TextEdit.Int(trimmedReply, trimNonNum, out failed);
                        if (failed)
                        {
                            errorReason = 2;
                        }
                        else
                        {
                            if (res != thr)
                            {
                                errorReason = 3;
                                failed = true;
                            }
                            else
                            {
                                failed = false;
                            }
                        }
                    }
                    break;
                case 4:
                    trimmedReply = TextEdit.WordRange(reply, line, start, start, separator, trimSeparator, out failed);
                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        int res = TextEdit.Int(trimmedReply, trimNonNum, out failed);
                        if (failed)
                        {
                            errorReason = 2;
                        }
                        else
                        {
                            if (res == thr)
                            {
                                errorReason = 3;
                                failed = true;
                            }
                            else
                            {
                                failed = false;
                            }
                        }
                    }
                    break;
                case 5:
                    trimmedReply = TextEdit.WordRange(reply, line, start, start, separator, trimSeparator, out failed);
                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        int res = TextEdit.Int(trimmedReply, trimNonNum, out failed);
                        if (failed)
                        {
                            errorReason = 2;
                        }
                        else
                        {
                            if (res <= thr)
                            {
                                errorReason = 3;
                                failed = true;
                            }
                            else
                            {
                                failed = false;
                            }
                        }
                    }
                    break;
                case 6:
                    trimmedReply = TextEdit.WordRange(reply, line, start, start, separator, trimSeparator, out failed);
                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        int res = TextEdit.Int(trimmedReply, trimNonNum, out failed);
                        if (failed)
                        {
                            errorReason = 2;
                        }
                        else
                        {
                            if (res >= thr)
                            {
                                errorReason = 3;
                                failed = true;
                            }
                            else
                            {
                                failed = false;
                            }
                        }
                    }
                    break;
                case 7:
                    int resu = TextEdit.LineCount(reply);
                    if (resu != thr)
                    {
                        errorReason = 3;
                        failed = true;
                    }
                    else failed = false;
                    trimmedReply = resu.ToString();
                    break;
                case 8:
                    int resul = TextEdit.LineCount(reply);
                    if (resul == thr)
                    {
                        errorReason = 3;
                        failed = true;
                    }
                    else failed = false;
                    trimmedReply = resul.ToString();
                    break;
                case 9:
                    int result = TextEdit.LineCount(reply);
                    if (result <= thr)
                    {
                        errorReason = 3;
                        failed = true;
                    }
                    else failed = false;
                    trimmedReply = result.ToString();
                    break;
                case 10:
                    int resultt = TextEdit.LineCount(reply);
                    if (resultt >= thr)
                    {
                        errorReason = 3;
                        failed = true;
                    }
                    else failed = false;
                    trimmedReply = resultt.ToString();
                    break;
                case 11:
                    trimmedReply = TextEdit.WordRange(reply, line, start, end, separator, trimSeparator, out failed);
                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        if (!trimmedReply.Contains(sub))
                        {
                            errorReason = 3;
                            failed = true;
                        }
                        else failed = false;
                    }
                    break;
                case 12:
                    trimmedReply = TextEdit.WordRange(reply, line, start, end, separator, trimSeparator, out failed);
                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        if (trimmedReply.Contains(sub))
                        {
                            errorReason = 3;
                            failed = true;
                        }
                        else failed = false;
                    }
                    break;
                case 13:
                    trimmedReply = TextEdit.LineRange(reply, start, end, out failed);

                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        if (!trimmedReply.Contains(sub))
                        {
                            errorReason = 3;
                            failed = true;
                        }
                        else failed = false;
                    }
                    break;

                case 14:
                    trimmedReply = TextEdit.LineRange(reply, start, end, out failed);

                    if (failed)
                    {
                        errorReason = 1;
                    }
                    else
                    {
                        if (trimmedReply.Contains(sub))
                        {
                            errorReason = 3;
                            failed = true;
                        }
                        else failed = false;
                    }
                    break;

            }

            cp = new ConditionPresentation(name, mode, failed, errorReason, trimmedReply, command, thr.ToString(), sub);
        }

    }



    public class ConditionPresentation
    {
        public Color backgroundColor { get; private set; }
        public string name { get; private set; }
        public string lines { get; private set; }

        public ConditionPresentation(string name, int mode, bool failed, int errorReason, string trimmedReply, string command, string thr = "", string substring = "")
        {
            this.name = name;
            Color g = Color.FromArgb("#BEF7A6");
            Color r = Color.FromArgb("#F5C4C9");
            string[] modes = new string[] { "EQ", "NEQ", "GT", "LT" };

            switch (mode)
            {
                case 0:
                    backgroundColor = g;
                    lines = "Commands executed.";
                    break;
                case 1:
                    backgroundColor = failed ? r : g;
                    lines = "Command: " + command + "\nReply: " + trimmedReply + "\n" + ErrorString(errorReason);
                    break;
                case 2:
                    backgroundColor = failed ? r : g;
                    lines = "Command: " + command + "\nReply: " + trimmedReply + "\n" + ErrorString(errorReason);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    backgroundColor = failed ? r : g;
                    lines = "Command: " + command + "\nReply: " + trimmedReply + ", Threshold: " + thr + ", Mode " + modes[mode-3] + "\n" + ErrorString(errorReason);
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    backgroundColor = failed ? r : g;
                    lines = "Command: " + command + "\nLine count: " + trimmedReply + ", Threshold: " + thr + ", Mode " + modes[mode - 7] + "\n" + ErrorString(errorReason);
                    break;
                case 11:
                case 12:
                    backgroundColor = failed ? r : g;
                    lines = "Command: " + command + "\nReply: " + trimmedReply + "\nSubstring: " + substring + ", Mode " + modes[mode - 11] + "\n" + ErrorString(errorReason);
                    break;
                case 13:
                case 14:
                    backgroundColor = failed ? r : g;
                    lines = "Command: " + command + "\nSubstring: " + substring + ", Mode " + modes[mode - 13] + "\n" + ErrorString(errorReason);
                    break;
            }
        }

        private string ErrorString(int errorReason)
        {
            if (errorReason == 0) return "Error reason: None\n";
            else if (errorReason == 1) return "Error reason: Index error (out of range) \n";
            else if (errorReason == 2) return "Error reason: Integer parse error (NaN) \n";
            else return "Error reason: Condition not matched \n";
        }
    }
}
