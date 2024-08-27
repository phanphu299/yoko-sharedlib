using System;
using System.Collections.Generic;

namespace AHI.Infrastructure.Service.Tag.Model
{
    public interface IUpsertTagCommand
    {
        bool IgnoreNotFound { get; set; }
        IEnumerable<UpsertTag> Tags { get; set; }
        Guid ApplicationId { get; set; }
        string Upn { get; set; }
    }
}