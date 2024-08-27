using AHI.Infrastructure.Service.Tag.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHI.Infrastructure.Service.Tag.SqlServer.Configuration
{
    public class EntityTagV2Configuration<T> : IEntityTypeConfiguration<T> where T : EntityTag
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            // configure the model.
            builder.ToTable("entity_tags");
            builder.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            builder.Property(x => x.TagId).HasColumnName("tag_id").IsRequired();
            builder.Property(x => x.EntityType).HasColumnName("entity_type").IsRequired();
            builder.Property(x => x.EntityIdString).HasColumnName("entity_id_varchar");
            builder.Property(x => x.EntityIdInt).HasColumnName("entity_id_int");
            builder.Property(x => x.EntityIdLong).HasColumnName("entity_id_long");
            builder.Property(x => x.EntityIdGuid).HasColumnName("entity_id_uuid");
            builder.Property(x => x.OrderNumber).HasColumnName("order_number");
        }
    }
}