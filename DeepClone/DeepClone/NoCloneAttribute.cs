using System;

namespace VIC.CloneExtension
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoCloneAttribute : Attribute
    {
    }
}