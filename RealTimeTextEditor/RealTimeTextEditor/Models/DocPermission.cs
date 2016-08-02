using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeTextEditor.Models
{
    public class DocPermission
    {
        public int ID { get; set; }
        public int DocumentID { get; set; }
        // email identifies the individual to whom read and/or edit permissions are to be granted for the specified document
        public string Email { get; set; }
        public bool Author { get; set; }
        public bool Read { get; set; }
        public bool Edit { get; set; }

        public virtual Document Document { get; set; }
    }
}