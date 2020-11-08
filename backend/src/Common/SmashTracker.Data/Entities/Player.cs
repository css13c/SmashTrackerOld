using System;
using System.ComponentModel.DataAnnotations;
using SmashTracker.Data.Interfaces;

namespace SmashTracker.Data.Entities
{
    public class Player : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
