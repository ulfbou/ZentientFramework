using System;
using System.Collections.Generic;
using System.Reflection;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Common.Metadata;
using Zentient.Metadata.Attributes;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Scans types and members for metadata attributes and produces IMetadata instances.
    /// </summary>
    public interface IAttributeMetadataScanner
    {
        IMetadata Scan(Type type);
        IMetadata Scan(MemberInfo member);
        IEnumerable<(MemberInfo member, IMetadata metadata)> ScanAll(Assembly assembly);
    }

    /// <summary>
    /// Converts attribute collections to IMetadata.
    /// </summary>
    public static class AttributeMetadataConverter
    {
        public static IMetadata Convert(IEnumerable<Attribute> attributes)
        {
            var builder = Zentient.Metadata.Metadata.Create();
            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case MetadataTagAttribute tag:
                        builder.SetTag(tag.Key, tag.TagValue);
                        break;
                    case CategoryDefinitionAttribute cat:
                        builder.SetTag("category", cat.Name);
                        break;
                    case DefinitionCategoryAttribute legacyCat:
                        builder.SetTag("category", legacyCat.CategoryName);
                        break;
                    case DefinitionTagAttribute legacyTag:
                        builder.SetTag("tags", legacyTag.Tags);
                        break;
                }
            }
            return builder.Build();
        }
    }
}
