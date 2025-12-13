using System;

namespace Minity.General
{
    public struct EnumIdentifier : IEquatable<EnumIdentifier>
    {
        public int Value;
        public Type Type;

        public string Name { get; private set; }

        public static EnumIdentifier Wrap<T>(T identifier) where T : Enum
        {
            return new EnumIdentifier()
            {
                // the hash code of an integer is itself
                // this avoids boxing to cast the value
                Value = identifier.GetHashCode(),
                Type = typeof(T)
            };
        }
        
        public static EnumIdentifier WrapType(Type type, Enum identifier)
        {
            return new EnumIdentifier()
            {
                Value = identifier.GetHashCode(),
                Type = type
            };
        }
        
        public static EnumIdentifier WrapReflection(Enum identifier)
        {
            var type = identifier.GetType();
            return new EnumIdentifier()
            {
                Value = identifier.GetHashCode(),
                Type = type
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is not EnumIdentifier other) return false;
            return Value == other.Value && Type == other.Type;
        }
        
        public bool Equals(EnumIdentifier other)
        {
            return Value == other.Value && Type == other.Type;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + Value;
                hash = hash * 31 + Type.GetHashCode();
                return hash;
            }
        }
        
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = Type.FullName + "." + Enum.GetName(Type, Value);
            }
            return Name;
        }
    }
}
