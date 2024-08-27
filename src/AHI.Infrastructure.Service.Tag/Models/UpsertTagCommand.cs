using System;
using System.Collections.Generic;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public class UpsertTagCommand : IUpsertTagCommand
    {
        public bool IgnoreNotFound { get; set; } = false;
        public IEnumerable<UpsertTag> Tags { get; set; }
        public Guid ApplicationId { get; set; }
        public string Upn { get; set; }
    }

    public class UpsertTag
    {
        public long? Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}