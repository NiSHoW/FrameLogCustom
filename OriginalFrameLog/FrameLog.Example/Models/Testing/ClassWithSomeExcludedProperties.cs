﻿using FrameLog.Filter;
using System.ComponentModel.DataAnnotations;

namespace FrameLog.Example.Models.Testing
{
    [MetadataType(typeof(Metadata))]
    public class ClassWithSomeExcludedProperties
    {
        [Key]
        public int Id { get; set; }

        [DoNotLog]
        public virtual User ExcludedNavigationProperty { get; set; }
        [DoNotLog]
        public string ExcludedScalarProperty { get; set; }

        public int PropertyExcludedByMetadata { get; set; }

        [DoNotLog]
        private int privateExcludedProperty { get; set; }
        public void SetPrivateExcludedProperty(int value)
        {
            this.privateExcludedProperty = value;
        }

        public class Metadata
        {
            [DoNotLog]
            public int PropertyExcludedByMetadata { get; set; }
        }
    }
}
