using System;

namespace Minity.General
{
    public struct EnumIdentifier
    {
        public int Value;
        public Type Type;

        public string Name { get; private set; }

        public static EnumIdentifier Wrap<T>(T identifier) where T : Enum
        {
            return new EnumIdentifier()
            {
                Value = (int)(object)identifier,
                Type = typeof(T)
            };
        }
        
        public static EnumIdentifier WrapType(Type type, Enum identifier)
        {
            return new EnumIdentifier()
            {
                Value = (int)(object)identifier,
                Type = type
            };
        }
        
        public static EnumIdentifier WrapReflection(Enum identifier)
        {
            var type = identifier.GetType();
            return new EnumIdentifier()
            {
                Value = (int)(object)identifier,
                Type = type
            };
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = Type.FullName + "." + Value;
            }
            return Name;
        }
    }
}
