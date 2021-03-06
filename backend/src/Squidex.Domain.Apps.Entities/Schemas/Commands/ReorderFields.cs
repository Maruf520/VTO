// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.Domain.Apps.Entities.Schemas.Commands
{
    public sealed class ReorderFields : ParentFieldCommand
    {
        public long[] FieldIds { get; set; }
    }
}
