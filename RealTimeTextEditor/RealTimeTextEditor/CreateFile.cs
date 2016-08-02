using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RealTimeTextEditor
{
    public class CreateFile
    {
        public bool Create(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // file create is wrapped in using statement to ensure file access is disposed of correctly
                using (var stream = File.Create(path));
                return (true);
            }

            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                return (false);
            }
        }

    }
}