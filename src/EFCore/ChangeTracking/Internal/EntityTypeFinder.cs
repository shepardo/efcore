// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    public class EntityTypeFinder : IEntityTypeFinder
    {
        private const string DefaultEntityTypeNamePropertyName = "__EntityTypeName__";

        private readonly IModel _model;

        public EntityTypeFinder([NotNull] EntityTypeFinderDependencies dependencies)
        {
            _model = dependencies.Model;
        }

        public virtual IEntityType FindEntityType(object entity)
        {
            Check.NotNull(entity, nameof(entity));

            var clrType = entity.GetType();
            var entityType = _model.FindRuntimeEntityType(clrType);
            if (entityType == null)
            {
                // entity could be a shared EntityType instance
                var indexerPropertyInfo = _model.AsModel().FindIndexerPropertyInfo(clrType);
                if (indexerPropertyInfo != null)
                {
                    try
                    {
                        var entityTypeName = (string)indexerPropertyInfo.GetValue(entity, new[] { DefaultEntityTypeNamePropertyName });

                        if (!string.IsNullOrWhiteSpace(entityTypeName))
                        {
                            entityType = _model.FindEntityType(entityTypeName);

                            if (!entityType.IsSharedType)
                            {
                                // TODO: throw?
                                return null;
                            }
                        }
                        else
                        {
                            // TODO: Throw for empty string as entityTypeName?
                        }
                    }
                    catch
                    {
                        // TODO: Add exception messages
                        // Key not present in indexer
                        // Value is not string
                    }
                }
            }

            return entityType;
        }
    }
}
