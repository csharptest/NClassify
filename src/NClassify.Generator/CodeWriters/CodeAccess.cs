using System;

namespace NClassify.Generator.CodeWriters
{
    [Flags]
    public enum CodeAccess
    {
        Private = 0x00,
        Public = 0x01,
        Protected = 0x02,
    }

    [Flags]
    public enum PropertyAccessors
    {
        Read = 0x04,
        Write = 0x08,
        ReadWrite = 0x0C,
    }
}
