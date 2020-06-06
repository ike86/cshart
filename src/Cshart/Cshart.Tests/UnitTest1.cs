using System;
using FluentAssertions;
using Xunit;

namespace Cshart.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Chart.Expand<UnitTest1>(x => x.Test1())
                .Should().Be(@"
System.Type.GetTypeFromHandle(System.RuntimeTypeHandle handle);
System.Linq.Expressions.Expression.Parameter(System.Type type, System.String name);
Cshart.Tests.UnitTest1.Test1();
System.Reflection.MethodBase.GetMethodFromHandle(System.RuntimeMethodHandle handle);
System.Array.Empty();
System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression instance, System.Reflection.MethodInfo method, System.Linq.Expressions.Expression[] arguments);
System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression body, System.Linq.Expressions.ParameterExpression[] parameters);
Cshart.Chart.Expand(System.Linq.Expressions.Expression`1[[System.Action`1[[Cshart.Tests.UnitTest1, Cshart.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]] selector);
FluentAssertions.AssertionExtensions.Should(System.String actualValue);
System.Array.Empty();
FluentAssertions.Primitives.StringAssertions.Be(System.String expected, System.String because, System.Object[] becauseArgs);");
        }
    }
}
