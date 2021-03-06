// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NodaTime;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.DefaultValues;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Json.Objects;
using Xunit;

namespace Squidex.Domain.Apps.Core.Operations.DefaultValues
{
    public class DefaultValuesTests
    {
        private readonly Instant now = Instant.FromUtc(2017, 10, 12, 16, 30, 10);
        private readonly LanguagesConfig languagesConfig = LanguagesConfig.English.Set(Language.DE);
        private readonly Language language = Language.DE;
        private readonly Schema schema;

        public DefaultValuesTests()
        {
            schema =
                new Schema("my-schema")
                    .AddString(1, "my-string", Partitioning.Language,
                        new StringFieldProperties { DefaultValue = "en-string" })
                    .AddNumber(2, "my-number", Partitioning.Invariant,
                        new NumberFieldProperties())
                    .AddDateTime(3, "my-datetime", Partitioning.Invariant,
                        new DateTimeFieldProperties { DefaultValue = now })
                    .AddBoolean(4, "my-boolean", Partitioning.Invariant,
                        new BooleanFieldProperties { DefaultValue = true });
        }

        [Fact]
        public void Should_enrich_with_default_values()
        {
            var data =
                new ContentData()
                    .AddField("my-string",
                        new ContentFieldData()
                            .AddLocalized("de", "de-string"))
                    .AddField("my-number",
                        new ContentFieldData()
                            .AddInvariant(456));

            data.GenerateDefaultValues(schema, languagesConfig.ToResolver());

            Assert.Equal(456, ((JsonNumber)data["my-number"]!["iv"]).Value);

            Assert.Equal("de-string", data["my-string"]!["de"].ToString());
            Assert.Equal("en-string", data["my-string"]!["en"].ToString());

            Assert.Equal(now.ToString(), data["my-datetime"]!["iv"].ToString());

            Assert.True(((JsonBoolean)data["my-boolean"]!["iv"]).Value);
        }

        [Fact]
        public void Should_also_enrich_with_default_values_when_string_is_empty()
        {
            var data =
                new ContentData()
                    .AddField("my-string",
                        new ContentFieldData()
                            .AddLocalized("de", string.Empty))
                    .AddField("my-number",
                        new ContentFieldData()
                            .AddInvariant(456));

            data.GenerateDefaultValues(schema, languagesConfig.ToResolver());

            Assert.Equal("en-string", data["my-string"]!["de"].ToString());
            Assert.Equal("en-string", data["my-string"]!["en"].ToString());
        }

        [Fact]
        public void Should_get_default_value_from_assets_field()
        {
            var field =
                Fields.Assets(1, "1", Partitioning.Invariant,
                    new AssetsFieldProperties());

            Assert.Equal(JsonValue.Array(), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_assets_field_when_set()
        {
            var field =
                Fields.Assets(1, "1", Partitioning.Invariant,
                    new AssetsFieldProperties { DefaultValue = new[] { "1", "2" } });

            Assert.Equal(JsonValue.Array("1", "2"), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_assets_field_when_localized()
        {
            var field =
                Fields.Assets(1, "1", Partitioning.Invariant,
                    new AssetsFieldProperties
                    {
                        DefaultValues = new LocalizedValue<string[]?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = new[] { "1", "2" }
                    });

            Assert.Equal(JsonValue.Array(), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_boolean_field()
        {
            var field =
                Fields.Boolean(1, "1", Partitioning.Invariant,
                    new BooleanFieldProperties { DefaultValue = true });

            Assert.Equal(JsonValue.True, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_boolean_field_when_localized()
        {
            var field =
                Fields.Boolean(1, "1", Partitioning.Invariant,
                    new BooleanFieldProperties
                    {
                        DefaultValues = new LocalizedValue<bool?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = true
                    });

            Assert.Equal(JsonValue.Null, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_datetime_field()
        {
            var field =
                Fields.DateTime(1, "1", Partitioning.Invariant,
                    new DateTimeFieldProperties { DefaultValue = FutureDays(15) });

            Assert.Equal(JsonValue.Create(FutureDays(15)), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_datetime_field_when_set_to_today()
        {
            var field =
                Fields.DateTime(1, "1", Partitioning.Invariant,
                    new DateTimeFieldProperties { CalculatedDefaultValue = DateTimeCalculatedDefaultValue.Today });

            Assert.Equal(JsonValue.Create("2017-10-12T00:00:00Z"), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_datetime_field_when_set_to_now()
        {
            var field =
                Fields.DateTime(1, "1", Partitioning.Invariant,
                    new DateTimeFieldProperties { CalculatedDefaultValue = DateTimeCalculatedDefaultValue.Now });

            Assert.Equal(JsonValue.Create("2017-10-12T16:30:10Z"), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_datetime_field_when_localized()
        {
            var field =
                Fields.DateTime(1, "1", Partitioning.Invariant,
                    new DateTimeFieldProperties
                    {
                        DefaultValues = new LocalizedValue<Instant?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = FutureDays(15)
                    });

            Assert.Equal(JsonValue.Null, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_json_field()
        {
            var field =
                Fields.Json(1, "1", Partitioning.Invariant,
                    new JsonFieldProperties());

            Assert.Equal(JsonValue.Null, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_geolocation_field()
        {
            var field =
                Fields.Geolocation(1, "1", Partitioning.Invariant,
                    new GeolocationFieldProperties());

            Assert.Equal(JsonValue.Null, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_number_field()
        {
            var field =
                Fields.Number(1, "1", Partitioning.Invariant,
                    new NumberFieldProperties { DefaultValue = 12 });

            Assert.Equal(JsonValue.Create(12), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_number_field_when_localized()
        {
            var field =
                Fields.Number(1, "1", Partitioning.Invariant,
                    new NumberFieldProperties
                    {
                        DefaultValues = new LocalizedValue<double?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = 12
                    });

            Assert.Equal(JsonValue.Null, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_references_field()
        {
            var field =
                Fields.References(1, "1", Partitioning.Invariant,
                    new ReferencesFieldProperties());

            Assert.Equal(JsonValue.Array(), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_references_field_when_set()
        {
            var field =
                Fields.References(1, "1", Partitioning.Invariant,
                    new ReferencesFieldProperties { DefaultValue = new[] { "1", "2" } });

            Assert.Equal(JsonValue.Array("1", "2"), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_references_field_when_localized()
        {
            var field =
                Fields.References(1, "1", Partitioning.Invariant,
                    new ReferencesFieldProperties
                    {
                        DefaultValues = new LocalizedValue<string[]?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = new[] { "1", "2" }
                    });

            Assert.Equal(JsonValue.Array(), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_string_field()
        {
            var field =
                Fields.String(1, "1", Partitioning.Invariant,
                    new StringFieldProperties { DefaultValue = "default" });

            Assert.Equal(JsonValue.Create("default"), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_string_field_when_localized()
        {
            var field =
                Fields.String(1, "1", Partitioning.Invariant,
                    new StringFieldProperties
                    {
                        DefaultValues = new LocalizedValue<string?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = "default"
                    });

            Assert.Equal(JsonValue.Null, DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_tags_field()
        {
            var field =
                Fields.Tags(1, "1", Partitioning.Invariant,
                    new TagsFieldProperties { DefaultValue = new[] { "tag1", "tag2" } });

            Assert.Equal(JsonValue.Array("tag1", "tag2"), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        [Fact]
        public void Should_get_default_value_from_tags_field_when_localized()
        {
            var field =
                Fields.Tags(1, "1", Partitioning.Invariant,
                    new TagsFieldProperties
                    {
                        DefaultValues = new LocalizedValue<string[]?>
                        {
                            [language.Iso2Code] = null
                        },
                        DefaultValue = new[] { "tag1", "tag2" }
                    });

            Assert.Equal(JsonValue.Array(), DefaultValueFactory.CreateDefaultValue(field, now, language.Iso2Code));
        }

        private Instant FutureDays(int days)
        {
            return now.WithoutMs().Plus(Duration.FromDays(days));
        }
    }
}
