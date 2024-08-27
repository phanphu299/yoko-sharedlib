using System;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public interface IEntityTag
    {
        long Id { get; set; }
        long TagId { get; set; }
        string EntityType { get; set; }
        string EntityIdString { get; set; }
        int? EntityIdInt { get; set; }
        long? EntityIdLong { get; set; }
        Guid? EntityIdGuid { get; set; }
        int OrderNumber { get; set; }
    }
}