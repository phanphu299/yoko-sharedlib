using AHI.Infrastructure.Service.Tag.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AHI.Infrastructure.Service.Tag.Extension
{
    public static class UpsertTagExtension
    {
        public static bool IsSameTags<TEntityDb>(this IUpsertTagCommand upsertTagCommand, ICollection<TEntityDb> currentTags) where TEntityDb : IEntityTag
        {
            var commandTags = upsertTagCommand.Tags?.Select(x => x.Id);
            var oldTags = currentTags?.OrderBy(x => x.Id).Select(x => (long?)x.TagId);
            var isSameTags = ((commandTags == null || !commandTags.Any()) && (oldTags == null || !oldTags.Any()))
                                || (commandTags != null && commandTags.SequenceEqual(oldTags));
            return isSameTags;
        }
    }
}