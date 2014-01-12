using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace SkyLinq.Composition
{
    public class CodeGenUtil
    {
        public static ConstructorInfo GetConstructorInfo<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body as NewExpression;
            if (body == null) throw new InvalidOperationException("Invalid expression form passed");

            return body.Constructor;
        }

        public static readonly OpCode[] ArgsOpcodes = {
            OpCodes.Ldarg_1,
            OpCodes.Ldarg_2,
            OpCodes.Ldarg_3
        };

        public static void EmitLoadArgument(ILGenerator il, int argumentNumber)
        {
            if (argumentNumber < ArgsOpcodes.Length)
            {
                il.Emit(ArgsOpcodes[argumentNumber]);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, argumentNumber + 1);
            }
        }
    }
}
