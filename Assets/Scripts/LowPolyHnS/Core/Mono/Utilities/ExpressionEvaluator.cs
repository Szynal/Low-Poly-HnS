using LowPolyHnS.Core.Math;

namespace LowPolyHnS.Core
{
    public static class ExpressionEvaluator
    {
        public static float Evaluate(string expression)
        {
            return Expression.Evaluate(expression);
        }
    }
}