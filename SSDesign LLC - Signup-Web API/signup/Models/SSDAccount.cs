using System;
using System.Data.Entity;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace signup.Models
{
    public class SSDCustomer : IDisposable
    {
        public string Salt { get; set; }
        public string Hash { get; set; }

        public int CID { get; set; }

        public string username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string CreatedTime { get; set; }

        public SSDCustomer()
        {
            Salt = "";
            Hash = "";

            CID = -1;

            username = "NULL";
            Firstname = "NULL";
            Lastname = "NULL";
            Email = "NULL";
            Enabled = false;
        }
        public SSDCustomer(string username, string firstname, string lastname, string email, bool Enabled) : this()
        {
            this.username = this.username;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Email = email;
            this.Enabled = Enabled;
        }

        public bool isNull()
        {
            return (string.IsNullOrEmpty(username));
        }

        void IDisposable.Dispose()
        {

        }
    }

    public class SSDAccountContext : DbContext
    {
        public DbSet<SSDCustomer> SSDAccounts { get; set; }
    }
}