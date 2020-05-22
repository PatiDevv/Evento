using System;
using System.Collections.Generic;
using System.Text;

namespace Evento.Core.Domain
{
    public class User : Entity
    { 
        public string Role { get; protected set; }
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string Password { get; protected set; }
        public DateTime CreateAt { get; protected set; }

        protected User()
        {
        }

        public User(Guid id, string role, string name, string email, string password)
        {
            Id = id;
            Role = role;
            Name = name;
            Email = email;
            Password = password;
            CreateAt = DateTime.UtcNow;
        }
    }
}
