using System;

namespace LowPolyHnS.Core.Math
{
    public class Parser
    {
        private readonly Tokenizer tokenizer;

        // INITIALIZER: ---------------------------------------------------------------------------

        public Parser(string expression)
        {
            tokenizer = new Tokenizer(expression);
        }

        public float Evaluate()
        {
            Node node = ParseExpression();
            if (node == null) return 0;
            return node.Evaluate();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Node ParseExpression()
        {
            Node node = ParseAddSubtract();
            if (tokenizer.currentToken != Tokenizer.Token.EOF)
            {
                throw new Exception("Unexpected characters at end of expression");
            }

            return node;
        }

        private Node ParseAddSubtract()
        {
            Node lhs = ParseMultiplyDivide();
            while (true)
            {
                Func<float, float, float> op = null;
                if (tokenizer.currentToken == Tokenizer.Token.Add)
                {
                    op = (a, b) => a + b;
                }
                else if (tokenizer.currentToken == Tokenizer.Token.Subtract)
                {
                    op = (a, b) => a - b;
                }

                if (op == null) return lhs;
                tokenizer.NextToken();

                Node rhs = ParseMultiplyDivide();
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        private Node ParseMultiplyDivide()
        {
            Node lhs = ParseUnary();
            while (true)
            {
                Func<float, float, float> op = null;
                if (tokenizer.currentToken == Tokenizer.Token.Multiply)
                {
                    op = (a, b) => a * b;
                }
                else if (tokenizer.currentToken == Tokenizer.Token.Divide)
                {
                    op = (a, b) => a / b;
                }

                if (op == null) return lhs;

                tokenizer.NextToken();
                Node rhs = ParseUnary();
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        private Node ParseUnary()
        {
            if (tokenizer.currentToken == Tokenizer.Token.Add)
            {
                tokenizer.NextToken();
                return ParseUnary();
            }

            if (tokenizer.currentToken == Tokenizer.Token.Subtract)
            {
                tokenizer.NextToken();
                var rhs = ParseUnary();
                return new NodeUnary(rhs, a => -a);
            }

            return ParseLeaf();
        }

        private Node ParseLeaf()
        {
            if (tokenizer.currentToken == Tokenizer.Token.Number)
            {
                Node node = new NodeNumber(tokenizer.number);
                tokenizer.NextToken();
                return node;
            }

            if (tokenizer.currentToken == Tokenizer.Token.OpenParens)
            {
                tokenizer.NextToken();
                Node node = ParseAddSubtract();

                if (tokenizer.currentToken != Tokenizer.Token.CloseParens)
                {
                    throw new Exception("Missing close parenthesis");
                }

                tokenizer.NextToken();
                return node;
            }

            throw new Exception("Unexpected token: " + tokenizer.currentToken);
        }
    }
}