namespace LowPolyHnS.Core.Math
{
    public class NodeNumber : Node
    {
        private readonly float number;

        // INITIALIZERS: --------------------------------------------------------------------------

        public NodeNumber(float number)
        {
            this.number = number;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Evaluate()
        {
            return number;
        }
    }
}