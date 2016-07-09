using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RealTimeTextEditor
{
    public class CreateFile
    {
        public bool Create (string path)
        {
           try { 
                 if (File.Exists(path))
                 {
                  File.Delete(path);
                   }

                   File.Create(path);
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