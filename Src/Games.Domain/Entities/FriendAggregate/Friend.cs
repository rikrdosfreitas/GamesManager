using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Games.Domain.Entities.Common;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Games.Domain.Entities.FriendAggregate
{
    public class Friend : AggregateRoot
    {
        internal Friend() { /* EF TEsts*/ }

        private Friend(Guid id, string name, string nickName, string email) : base(id)
        {
            Name = name;
            NickName = nickName;
            Email = email;
        }

        [StringLength(100)]
        public string Name { get; private set; }

        [StringLength(100)]
        public string NickName { get; private set; }

        [StringLength(100)]
        public string Email { get; private set; }

        public static Friend Create(Guid id, string name, string nickName, string email)
        {
            return new Friend(id, name, nickName,email);
        }

        public virtual void Update(string name, string nickName, string email, byte[] ver)
        {
            Name = name;
            NickName = nickName;
            Email = email;

            base.Update(ver);
        }
    }
}
