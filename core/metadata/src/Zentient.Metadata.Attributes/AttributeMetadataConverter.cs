using System;
using System.Collections.Generic;
using System.Reflection;
using Zentient.Abstractions.Metadata;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Provides conversion from attribute collections to IMetadata.
    /// </summary>
    public static class AttributeMetadataConverter
    {
        public static IMetadata Convert(IEnumerable<Attribute> attributes)
        {
            // Use the same logic as MetadataAttributeReader, but for arbitrary attribute collections
            var builder = Metadata.Create();
            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case BehaviorDefinitionAttribute:
                        builder.SetTag(attribute.GetType().FullName!, Metadata.Empty);
                        break;
                    case CategoryDefinitionAttribute categoryAttribute:
                        builder.SetTag(attribute.GetType().FullName!, categoryAttribute.Name);
                        break;
                    case MetadataTagAttribute tagAttribute:
                        builder.SetTag(tagAttribute.Key, tagAttribute.TagValue);
                        break;
                    case Zentient.Abstractions.Common.Metadata.DefinitionCategoryAttribute legacyCat:
                        builder.SetTag("category", legacyCat.CategoryName);
                        break;
                    case Zentient.Abstractions.Common.Metadata.DefinitionTagAttribute legacyTag:
                        builder.SetTag("tags", legacyTag.Tags);
                        break;
                    default:
                        break;
                }
            }
            return builder.Build();
        }
    }
}
