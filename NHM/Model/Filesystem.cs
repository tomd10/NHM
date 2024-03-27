using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NHM.Model
{
    public static class Filesystem
    {
        public static async Task<string> ReadFile()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { ".nhmcfg" } }, // UTType values
                    { DevicePlatform.Android, new[] { ".nhmcfg" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".nhmcfg" } } // file extension
                });

            PickOptions options = new()
            {
                PickerTitle = "Please select a config file",
                FileTypes = customFileType,
            };
            try
            {
                var result = await FilePicker.Default.PickAsync(null);
                if (result != null)
                {
                    string text = File.ReadAllText(result.FullPath);
                    return text;
                }
                return null;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                return null;
            }


        }
    }
}
