using System;
using Microsoft.AspNet.SignalR;
using System.IO;

namespace RealTimeTextEditor
{
    public class DeltaHub : Hub
    {
        // send method forwards all text editor changes to other clients than the author of the change
        public void Send(string change)
        {
            Clients.Others.update(change);
        }

        // load method retrieves document contents from file 
        public void Load(string filename)
        {
            // insert load code here
            var path = AppDomain.CurrentDomain.BaseDirectory + "docs/" + filename + ".txt";

            var contents = File.ReadAllLines(path);
            Clients.Caller.replace(contents);
        }

        // save method writes the editor contents to a file
        public void Save(string filename, string contents)
        {
            // simple code to write to text file
            var path = AppDomain.CurrentDomain.BaseDirectory + "docs/" + filename + ".txt";

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteAsync(contents);
                }
            }

            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

            // inform all clients of successful save
            Clients.All.saved();
        }
    }
}