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
            public string doc { get; set; }
        }

        // list for keeping track of current users
        // this object must be static since the hub object is only instantiated whenever a connection event occurs 
        public static List<HubUser> HubUsers = new List<HubUser>();


        // join method registers users with all clients in group and registers already connected users on joining client
        public void Join(string id, string name, string colour, string doc)
            {
            // add user to group numbered by doc id
            JoinGroup(doc);
            // find other group members present
            var groupUsers = HubUsers.FindAll(s => s.doc == doc);
            var count = groupUsers.Count;
            if (count > 0)
            {
                for (int i = 0, n = count; i < n; i++)
                {
                    HubUser loggedIn = groupUsers[i];
                    Clients.Caller.register(loggedIn.id, loggedIn.name, loggedIn.colour);
                }
                Clients.OthersInGroup(doc).register(id, name, colour);
                // more efficient to package all the registrations into one message?
            }
            var newUser = new HubUser();
            newUser.connId = Context.ConnectionId;
            newUser.id = id;
            newUser.name = name;
            newUser.colour = colour;
            newUser.doc = doc;
            HubUsers.Add(newUser);

            // with these tasks complete we are ready to call the load rpc so inform the client
            Clients.Caller.ok_to_load();
        }

        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        // send method forwards all text editor changes to clients other than the author of the change
        public void Send(string change, string doc)
        {
            Clients.OthersInGroup(doc).update(change);
        }

        // load method retrieves document contents from file 
        public void Load(string filename, string doc)
        {
            var groupUsers = HubUsers.FindAll(s => s.doc == doc);
            if (groupUsers.Count == 1)
            { 
                // if the user is the only one accessing the document, load from saved file
                var path = AppDomain.CurrentDomain.BaseDirectory + "docs/" + filename + ".txt";
               try {
                    var contents = File.ReadAllLines(path);
                    Clients.Caller.replace(contents);
               }
                catch
                {
                    Console.Write("error reading file");
                } 
            }
            else
            {
            // if there are other users already editing, retrieve editor contents from first user
            HubUser masterCopy = groupUsers[0]; 
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
        public void Save(string filename, string contents, string doc)
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
                    sw.Write(contents);
                }
            }

            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

            // inform all clients of successful save
            Clients.Group(doc).saved();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // ensure that list of active users is up to date by removing disconecting users
            HubUser dcUser = HubUsers.Find(r => r.connId == Context.ConnectionId);
            var doc = dcUser.doc;
            HubUsers.Remove(dcUser);
            Clients.OthersInGroup(doc).disconnect(dcUser.name);
            return base.OnDisconnected(stopCalled);
        }
    }
}