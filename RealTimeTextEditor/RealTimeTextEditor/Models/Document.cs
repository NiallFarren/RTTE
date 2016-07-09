using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RealTimeTextEditor.Models
{
    public class Document
    {

        public int ID { get; set; }
        public string UserID { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public Boolean Public { get; set; }
        [Display(Name = "Created At")]
        private DateTime Date = DateTime.Now;

        [Display(Name = "Created At")]
        public DateTime _Date
        {
            get
            {
                return Date;
            }
            set
            {
                Date = value;
            }
        }

        public virtual ICollection<DocPermission> DocPermissions { get; set; }

    }
}