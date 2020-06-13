using System;
using System.Reflection;

namespace Cshart.Tests
{
    internal class FakeMemberInfo : MemberInfo
    {
        public FakeMemberInfo(string name)
        {
            Name = name;
        }

        public override Type DeclaringType => throw new NotImplementedException();

        public override MemberTypes MemberType => throw new NotImplementedException();

        public override string Name { get; }

        public override Type ReflectedType => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes(null, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}
