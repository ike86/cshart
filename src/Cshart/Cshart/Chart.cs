using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Reflection;

namespace Cshart
{
    public class Chart
    {
        public static string Expand<T>(Expression<Action<T>> selector)
        {
            var result = string.Empty;

            var expression = (MethodCallExpression)selector.Body;
            string name = expression.Method.Name;

            var methodBase = typeof(T).GetMethod(name);
            foreach (var instruction in methodBase.GetInstructions())
            {
                var methodInfo = instruction.Operand as MethodInfo;

                if (methodInfo != null)
                {
                    var declaringType = methodInfo.DeclaringType;
                    var parameters = methodInfo.GetParameters();

                    result +=
                        Environment.NewLine
                        + string.Format(
                            "{0}.{1}({2});",
                            declaringType.FullName,
                            methodInfo.Name,
                            string.Join(
                                ", ",
                                parameters
                                .Select(p => p.ParameterType.FullName + " " + p.Name)
                                .ToArray()));
                }
            }

            return result;
        }
    }
}
