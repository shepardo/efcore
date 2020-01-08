// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;

// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    public class EntityTypeFinderTest
    {
        [Fact]
        public void Can_find_EntityType_using_default_EntityTypeNamePropertyName()
        {
            IMutableModel model = new Model();
            var entityType = model.AddEntityType("TestDictionaryEntityType", typeof(Dictionary<string, object>));
            var idProperty = entityType.AddIndexedProperty("Id", typeof(int));
            idProperty.IsNullable = false;
            var propA = entityType.AddIndexedProperty("PropA", typeof(string));

            var instance = new Dictionary<string, object>()
            {
                { "__EntityTypeName__", "TestDictionaryEntityType" },
                { "Id", 0 },
                { "PropA", "PropAValue"},
            };

            var finder = new EntityTypeFinder(new EntityTypeFinderDependencies(model));

            Assert.Equal(entityType, finder.FindEntityType(instance));
        }

        [Fact]
        public void Return_null_if_no_matching_EntityType_found()
        {
            IMutableModel model = new Model();
            var entityType = model.AddEntityType("DifferentlyNamedEntityType", typeof(Dictionary<string, object>));
            var idProperty = entityType.AddIndexedProperty("Id", typeof(int));
            idProperty.IsNullable = false;
            var propA = entityType.AddIndexedProperty("PropA", typeof(string));

            var instance = new Dictionary<string, object>()
            {
                { "__EntityTypeName__", "TestDictionaryEntityType" },
                { "Id", 0 },
                { "PropA", "PropAValue"},
            };

            var finder = new EntityTypeFinder(new EntityTypeFinderDependencies(model));

            Assert.Null(finder.FindEntityType(instance));
        }
        [Fact]
        public void Return_null_if_find_non_shared_type_EntityType_with_same_name()
        {
            IMutableModel model = new Model();
            var entityType = model.AddEntityType(typeof(NonSharedTypeEntity));
            var idProperty = entityType.AddProperty("Id", typeof(int));
            idProperty.IsNullable = false;
            entityType.AddProperty("PropA", typeof(string));

            var instance = new Dictionary<string, object>()
            {
                { "__EntityTypeName__", entityType.Name },
                { "Id", 0 },
                { "PropA", "PropAValue"},
            };

            var finder = new EntityTypeFinder(new EntityTypeFinderDependencies(model));

            Assert.Null(finder.FindEntityType(instance));
        }

        private class NonSharedTypeEntity
        {
            public int Id { get; set; }
            public string PropA { get; set; }
        }
    }
}

