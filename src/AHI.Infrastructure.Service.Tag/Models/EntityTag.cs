using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public class EntityTag :  IEntity<long>, IEntityTag
    {
        public long Id { get; set; }
        public long TagId { get; set; }
        public string EntityType { get; set; }
        public string EntityIdString { get; set; }
        public int? EntityIdInt { get; set; }
        public long? EntityIdLong { get; set; }
        public Guid? EntityIdGuid { get; set; }
        public int OrderNumber { get; set; }
    }
}