using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using RealTimeTextEditor.Models;

namespace RealTimeTextEditor.Context
{
    public class RTTEContext : DbContext
    {

        public RTTEContext()
            : base("RTTEContext")
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocPermission> DocPermissions { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }


    }
}