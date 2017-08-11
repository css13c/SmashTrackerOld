using System;
using SmashTracker.Data.Interfaces;

namespace SmashTracker.Data.Entities
{
    public class GamePlayerCharacter : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        public Guid GamePlayerAssociationId { get; set; }
        public GamePlayerAssociation GamePlayer { get; set; }

        public Guid GameCharacterId { get; set; }
        public GameCharacter Character { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
