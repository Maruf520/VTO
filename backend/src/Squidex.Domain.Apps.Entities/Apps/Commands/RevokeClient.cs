// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.Domain.Apps.Entities.Apps.Commands
{
    public sealed class RevokeClient : AppUpdateCommand
    {
        public string Id { get; set; }
    }
}
