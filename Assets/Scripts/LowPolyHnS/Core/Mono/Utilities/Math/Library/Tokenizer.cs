using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LowPolyHnS.Core.Math
{
    public class Tokenizer
    {
        public enum Token
        {
            EOF,
            Add,
            Subtract,
            Multiply,
            Divide,
            OpenParens,
            CloseParens,
            Number
        }

        private static readonly CultureInfo CULTURE = new CultureInfo("en-US");
        private const char CHAR_EOF = '\0';
        private const char CHAR_PLUS = '+';
        private const char CHAR_MINUS = '-';
        private const char CHAR_MULT = '*';
        private const char CHAR_DIV = '/';
        private const char CHAR_S_PARENTHESIS = '(';
        private const char CHAR_E_PARENTHESIS = ')';
        private const char CHAR_DOT = '.';

        // PROPERTIES: ----------------------------------------------------------------------------

        private TextReader reader;

        public char currentCharacter { get; private set; }
        public Token currentToken { get; private set; }
        public float number { get; private set; }
        public string identifier { get; private set; }

        // INITIALIZER: ---------------------------------------------------------------------------

        public Tokenizer(string expression)
        {
            reader = new StringReader(expression);

            NextChar();
            NextToken();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void NextChar()
        {
            int character = reader.Read();
            currentCharacter = character < 0 ? CHAR_EOF : (char) character;
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(currentCharacter)) NextChar();
            switch (currentCharacter)
            {
                case CHAR_EOF:
                    currentToken = Token.EOF;
                    return;

                case CHAR_PLUS:
                    NextChar();
                    currentToken = Token.Add;
                    return;

                case CHAR_MINUS:
                    NextChar();
                    currentToken = Token.Subtract;
                    return;

                case CHAR_MULT:
                    NextChar();
                    currentToken = Token.Multiply;
                    return;

                case CHAR_DIV:
                    NextChar();
                    currentToken = Token.Divide;
                    return;

                case CHAR_S_PARENTHESIS:
                    NextChar();
                    currentToken = Token.OpenParens;
                    return;

                case CHAR_E_PARENTHESIS:
                    NextChar();
                    currentToken = Token.CloseParens;
                    return;
            }

            if (char.IsDigit(currentCharacter) || currentCharacter == CHAR_DOT)
            {
                var sb = new StringBuilder();
                bool hasFloatingPoint = false;
                while (char.IsDigit(currentCharacter) || !hasFloatingPoint && currentCharacter == CHAR_DOT)
                {
                    sb.Append(currentCharacter);
                    hasFloatingPoint = currentCharacter == CHAR_DOT;
                    NextChar();
                }

                number = float.Parse(sb.ToString(), CULTURE);
                currentToken = Token.Number;
                return;
            }

            throw new Exception(string.Format("Unexpected character: {0}", currentCharacter));
        }
    }
}