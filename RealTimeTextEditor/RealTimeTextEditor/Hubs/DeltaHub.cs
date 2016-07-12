using System;
using Microsoft.AspNet.SignalR;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTimeTextEditor
{
    public class DeltaHub : Hub
    {
        public class HubUser
        {
            public string connId { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string colour { get; set; }
        }

        // list for keeping track of current users
        // this object must be static since the hub object is only instantiated whenever a connection event occurs 
        public static List<HubUser> HubUsers = new List<HubUser>();


        // join method registers joining users with all clients and registers already connected users on joining client
        public void Join(string id, string name, string colour)
            {
                var count = HubUsers.Count;
                if (count > 0)
            {
                for (int i = 0, n = count; i < n; i++)
                {
                    HubUser loggedIn = HubUsers[i];
                    Clients.Caller.register(loggedIn.id, loggedIn.name, loggedIn.colour);
                }
                Clients.Others.register(id, name, colour);
                // more efficient to package all the registrations into one message?
            }
            var newUser = new HubUser();
            newUser.connId = Context.ConnectionId;
            newUser.id = id;
            newUser.name = name;
            newUser.colour = colour;
            HubUsers.Add(newUser);
        }

        // send method forwards all text editor changes to clients other than the author of the change
        public void Send(string change)
        {
            Clients.Others.update(change);
        }

        // load method retrieves document contents from file 
        public void Load(string filename)
        {
            if (HubUsers.Count == 1)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + "docs/" + filename + ".txt";
                var contents = File.ReadAllLines(path);
                Clients.Caller.replace(contents);
            }
            else
            {
            // retrieve editor contents from first user
            HubUser masterCopy = HubUsers[0]; // what if first user has disconnected?
            var requesterId = Context.ConnectionId;
            Clients.Client(masterCopy.connId).retrieve(requesterId);
            }
        }
        
        // sync method ensures that joining users get an up-to-date version
        public void Sync(string connId, string contents)
        {
            Clients.Client(connId).replace(contents);
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

        public override Task OnDisconnected(bool stopCalled)
        {
            HubUser dcUser = HubUsers.Find(r => r.connId == Context.ConnectionId);
            HubUsers.Remove(dcUser);
            Clients.Others.disconnect(dcUser.name);
            return base.OnDisconnected(stopCalled);
        }
    }
}