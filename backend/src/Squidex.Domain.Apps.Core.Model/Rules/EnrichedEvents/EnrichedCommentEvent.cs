// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Runtime.Serialization;
using Squidex.Shared.Users;

#pragma warning disable CA1822 // Mark members as static

namespace Squidex.Domain.Apps.Core.Rules.EnrichedEvents
{
    public sealed class EnrichedCommentEvent : EnrichedUserEventBase
    {
        public string Text { get; set; }

        public Uri? Url { get; set; }

        [IgnoreDataMember]
        public IUser MentionedUser { get; set; }

        [IgnoreDataMember]
        public override long Partition
        {
            get { return MentionedUser.Id.GetHashCode(); }
        }

        public bool ShouldSerializeMentionedUser()
        {
            return false;
        }

        public bool ShouldSerializePartition()
        {
            return false;
        }
    }
}
