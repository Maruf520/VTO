// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using FakeItEasy;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Core.TestHelpers;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Domain.Apps.Entities.TestHelpers;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Xunit;

#pragma warning disable SA1310 // Field names must not contain underscore

namespace Squidex.Domain.Apps.Entities.Apps.DomainObject.Guards
{
    public class GuardAppPatternsTests : IClassFixture<TranslationsFixture>
    {
        private readonly DomainId patternId = DomainId.NewGuid();
        private readonly AppPatterns patterns_0 = AppPatterns.Empty;

        [Fact]
        public void CanAdd_should_throw_exception_if_name_empty()
        {
            var command = new AddPattern { PatternId = patternId, Name = string.Empty, Pattern = ".*" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanAdd(command, App(patterns_0)),
                new ValidationError("Name is required.", "Name"));
        }

        [Fact]
        public void CanAdd_should_throw_exception_if_pattern_empty()
        {
            var command = new AddPattern { PatternId = patternId, Name = "any", Pattern = string.Empty };

            ValidationAssert.Throws(() => GuardAppPatterns.CanAdd(command, App(patterns_0)),
                new ValidationError("Pattern is required.", "Pattern"));
        }

        [Fact]
        public void CanAdd_should_throw_exception_if_pattern_not_valid()
        {
            var command = new AddPattern { PatternId = patternId, Name = "any", Pattern = "[0-9{1}" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanAdd(command, App(patterns_0)),
                new ValidationError("Pattern is not a valid value.", "Pattern"));
        }

        [Fact]
        public void CanAdd_should_throw_exception_if_name_exists()
        {
            var patterns_1 = patterns_0.Add(DomainId.NewGuid(), "any", "[a-z]", "Message");

            var command = new AddPattern { PatternId = patternId, Name = "any", Pattern = ".*" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanAdd(command, App(patterns_1)),
                new ValidationError("A pattern with the same name already exists."));
        }

        [Fact]
        public void CanAdd_should_throw_exception_if_pattern_exists()
        {
            var patterns_1 = patterns_0.Add(DomainId.NewGuid(), "any", "[a-z]", "Message");

            var command = new AddPattern { PatternId = patternId, Name = "other", Pattern = "[a-z]" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanAdd(command, App(patterns_1)),
                new ValidationError("This pattern already exists but with another name."));
        }

        [Fact]
        public void CanAdd_should_not_throw_exception_if_command_is_valid()
        {
            var command = new AddPattern { PatternId = patternId, Name = "any", Pattern = ".*" };

            GuardAppPatterns.CanAdd(command, App(patterns_0));
        }

        [Fact]
        public void CanDelete_should_throw_exception_if_pattern_not_found()
        {
            var command = new DeletePattern { PatternId = patternId };

            Assert.Throws<DomainObjectNotFoundException>(() => GuardAppPatterns.CanDelete(command, App(patterns_0)));
        }

        [Fact]
        public void CanDelete_should_not_throw_exception_if_command_is_valid()
        {
            var patterns_1 = patterns_0.Add(patternId, "any", ".*", "Message");

            var command = new DeletePattern { PatternId = patternId };

            GuardAppPatterns.CanDelete(command, App(patterns_1));
        }

        [Fact]
        public void CanUpdate_should_throw_exception_if_name_empty()
        {
            var patterns_1 = patterns_0.Add(patternId, "any", ".*", "Message");

            var command = new UpdatePattern { PatternId = patternId, Name = string.Empty, Pattern = ".*" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanUpdate(command, App(patterns_1)),
                new ValidationError("Name is required.", "Name"));
        }

        [Fact]
        public void CanUpdate_should_throw_exception_if_pattern_empty()
        {
            var patterns_1 = patterns_0.Add(patternId, "any", ".*", "Message");

            var command = new UpdatePattern { PatternId = patternId, Name = "any", Pattern = string.Empty };

            ValidationAssert.Throws(() => GuardAppPatterns.CanUpdate(command, App(patterns_1)),
                new ValidationError("Pattern is required.", "Pattern"));
        }

        [Fact]
        public void CanUpdate_should_throw_exception_if_pattern_not_valid()
        {
            var patterns_1 = patterns_0.Add(patternId, "any", ".*", "Message");

            var command = new UpdatePattern { PatternId = patternId, Name = "any", Pattern = "[0-9{1}" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanUpdate(command, App(patterns_1)),
                new ValidationError("Pattern is not a valid value.", "Pattern"));
        }

        [Fact]
        public void CanUpdate_should_throw_exception_if_name_exists()
        {
            var id1 = DomainId.NewGuid();
            var id2 = DomainId.NewGuid();

            var patterns_1 = patterns_0.Add(id1, "Pattern1", "[0-5]", "Message");
            var patterns_2 = patterns_1.Add(id2, "Pattern2", "[0-4]", "Message");

            var command = new UpdatePattern { PatternId = id2, Name = "Pattern1", Pattern = "[0-4]" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanUpdate(command, App(patterns_2)),
                new ValidationError("A pattern with the same name already exists."));
        }

        [Fact]
        public void CanUpdate_should_throw_exception_if_pattern_exists()
        {
            var id1 = DomainId.NewGuid();
            var id2 = DomainId.NewGuid();

            var patterns_1 = patterns_0.Add(id1, "Pattern1", "[0-5]", "Message");
            var patterns_2 = patterns_1.Add(id2, "Pattern2", "[0-4]", "Message");

            var command = new UpdatePattern { PatternId = id2, Name = "Pattern2", Pattern = "[0-5]" };

            ValidationAssert.Throws(() => GuardAppPatterns.CanUpdate(command, App(patterns_2)),
                new ValidationError("This pattern already exists but with another name."));
        }

        [Fact]
        public void CanUpdate_should_throw_exception_if_pattern_does_not_exists()
        {
            var command = new UpdatePattern { PatternId = patternId, Name = "Pattern1", Pattern = ".*" };

            Assert.Throws<DomainObjectNotFoundException>(() => GuardAppPatterns.CanUpdate(command, App(patterns_0)));
        }

        [Fact]
        public void CanUpdate_should_not_throw_exception_if_pattern_exist_with_valid_command()
        {
            var patterns_1 = patterns_0.Add(patternId, "any", ".*", "Message");

            var command = new UpdatePattern { PatternId = patternId, Name = "Pattern1", Pattern = ".*" };

            GuardAppPatterns.CanUpdate(command, App(patterns_1));
        }

        private static IAppEntity App(AppPatterns patterns)
        {
            var app = A.Fake<IAppEntity>();

            A.CallTo(() => app.Patterns)
                .Returns(patterns);

            return app;
        }
    }
}
