using System;

namespace LowPolyHnS.Core.Math
{
    public class NodeUnary : Node
    {
        private readonly Node rhs;
        private readonly Func<float, float> op;

        // INITIALIZER: ---------------------------------------------------------------------------

        public NodeUnary(Node rhs, Func<float, float> op)
        {
            this.rhs = rhs;
            this.op = op;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Evaluate()
        {
            float rhsVal = rhs.Evaluate();
            return op(rhsVal);
        }
    }
}