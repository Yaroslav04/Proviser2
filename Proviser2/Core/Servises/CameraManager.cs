using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Proviser2.Core.Servises
{
    public static class CameraManager
    {
        public static async Task TakePhotoAsync(string _case)
        {

           
           

            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo, _case);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {

            }
        }

        static async Task LoadPhotoAsync(FileResult photo, string _case)
        {
            if (photo == null)
            {
                return;
            }

            if (!Directory.Exists(Path.Combine(FileManager.GeneralPath("Capture"), _case.Replace("/", "."))))
            {
                Directory.CreateDirectory(Path.Combine(FileManager.GeneralPath("Capture"), _case.Replace("/", ".")));
            }

            var newFile = Path.Combine(Path.Combine(FileManager.GeneralPath("Capture"), _case.Replace("/", ".")), photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);
                }
            }       
        }
    }
}
