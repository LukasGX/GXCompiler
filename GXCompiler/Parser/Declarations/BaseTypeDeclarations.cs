using GXCompiler.GXLexer;

namespace GXCompiler.GXParser;

public partial class Parser
{
    private VariableDeclarationNode ParseStrDeclaration()
    {
        Advance();

        Token identifier = Expect(TokenType.Identifier);
        Expect(TokenType.Assign);

        List<Token> expressionTokens = [];

        while (Current.TokenType != TokenType.Semicolon && Current.TokenType != TokenType.EOF)
        {
            if (expressionTokens.Count == 0 ||
                expressionTokens.Last().TokenType == TokenType.Plus
            )
            {
                expressionTokens.Add(Expect(TokenType.Literal, TokenType.Identifier));
            }
            else
            {
                expressionTokens.Add(Expect(TokenType.Plus));
            }
        }

        Expect(TokenType.Semicolon);

        if (expressionTokens.Count == 0)
            throw new Exception("Expression cannot be empty");

        if (expressionTokens.Last().TokenType == TokenType.Plus)
            throw new Exception("Expected Semicolon, got Plus");

        if (expressionTokens.Count == 1 &&
            expressionTokens[0].TokenType == TokenType.Literal)
        {
            return new VariableDeclarationNode(
                TokenType.Str.ToString(),
                identifier.Value,
                new LiteralNode(expressionTokens[0].Value)
            );
        }
        else if (expressionTokens.Count == 1 &&
                 expressionTokens[0].TokenType == TokenType.Identifier)
        {
            return new VariableDeclarationNode(
                TokenType.Str.ToString(),
                identifier.Value,
                new IdentifierNode(expressionTokens[0].Value)
            );
        }
        else if (expressionTokens.Count > 1)
        {
            BinaryExpressionNode bxn = new("+");

            foreach (Token tk in expressionTokens)
            {
                if (tk.TokenType == TokenType.Literal)
                    bxn.Values.Add(new LiteralNode(tk.Value));
                else if (tk.TokenType == TokenType.Identifier)
                    bxn.Values.Add(new IdentifierNode(tk.Value));
                else if (tk.TokenType == TokenType.Plus)
                    continue;
                else
                    throw new Exception($"Unexpected token: {tk.TokenType}");
            }

            return new VariableDeclarationNode(
                TokenType.Str.ToString(),
                identifier.Value,
                bxn
            );
        }
        else
            throw new Exception("Broken expression");
    }

    private VariableDeclarationNode ParseNumDeclaration()
    {
        Token btk = Advance();

        Token identifier = Expect(TokenType.Identifier);
        Expect(TokenType.Assign);

        List<Token> expressionTokens = [];

        while (Current.TokenType != TokenType.Semicolon && Current.TokenType != TokenType.EOF)
        {
            expressionTokens.Add(Expect(
                TokenType.Number,
                TokenType.Identifier,
                TokenType.LeftParen,
                TokenType.RightParen,
                TokenType.Plus,
                TokenType.Minus,
                TokenType.Multiply,
                TokenType.Divide,
                TokenType.Power
            ));
        }

        Expect(TokenType.Semicolon);

        if (expressionTokens.Count == 0)
            throw new Exception("Expression cannot be empty");

        if (expressionTokens.Count == 1 &&
            expressionTokens[0].TokenType == TokenType.Number)
        {
            return new VariableDeclarationNode(
                btk.TokenType.ToString(),
                identifier.Value,
                new NumberNode(expressionTokens[0].Value)
            );
        }
        else if (expressionTokens.Count == 1 &&
            expressionTokens[0].TokenType == TokenType.Identifier)
        {
            return new VariableDeclarationNode(
                btk.TokenType.ToString(),
                identifier.Value,
                new IdentifierNode(expressionTokens[0].Value)
            );
        }
        else if (expressionTokens.Count > 1)
        {
            ExpressionSequenceNode xsn = new();

            foreach (Token tk in expressionTokens)
            {
                if (tk.TokenType == TokenType.Number)
                    xsn.Values.Add(new NumberNode(tk.Value));
                else if (tk.TokenType == TokenType.Identifier)
                    xsn.Values.Add(new IdentifierNode(tk.Value));
                else if (tk.TokenType == TokenType.LeftParen)
                    xsn.Values.Add(new LeftParenNode());
                else if (tk.TokenType == TokenType.RightParen)
                    xsn.Values.Add(new RightParenNode());
                else if (tk.TokenType == TokenType.Plus)
                    xsn.Values.Add(new PlusNode());
                else if (tk.TokenType == TokenType.Minus)
                    xsn.Values.Add(new MinusNode());
                else if (tk.TokenType == TokenType.Multiply)
                    xsn.Values.Add(new MultiplyNode());
                else if (tk.TokenType == TokenType.Divide)
                    xsn.Values.Add(new DivideNode());
                else if (tk.TokenType == TokenType.Power)
                    xsn.Values.Add(new PowerNode());
                else
                    throw new Exception($"Unexpected token: {tk.TokenType}");
            }

            return new VariableDeclarationNode(
                btk.TokenType.ToString(),
                identifier.Value,
                xsn
            );
        }
        else
            throw new Exception("Broken expression");
    }

    private VariableDeclarationNode ParseBoolDeclaration()
    {
        Advance();

        Token identifier = Expect(TokenType.Identifier);
        Expect(TokenType.Assign);

        List<Token> expressionTokens = [];

        while (Current.TokenType != TokenType.Semicolon && Current.TokenType != TokenType.EOF)
        {
            expressionTokens.Add(Expect(
                TokenType.BoolLiteral,
                TokenType.Identifier,
                TokenType.Not
            ));
        }

        Expect(TokenType.Semicolon);

        if (expressionTokens.Count == 0)
            throw new Exception("Expression cannot be empty");

        if (expressionTokens.Count == 1 &&
            expressionTokens[0].TokenType == TokenType.BoolLiteral)
        {
            if (!bool.TryParse(expressionTokens[0].Value, out bool val))
                throw new Exception($"{expressionTokens[0].Value} is not a valid bool literal");

            return new VariableDeclarationNode(
                TokenType.Bool.ToString(),
                identifier.Value,
                new BoolLiteralNode(val)
            );
        }
        else if (expressionTokens.Count == 1 &&
                 expressionTokens[0].TokenType == TokenType.Identifier)
        {
            return new VariableDeclarationNode(
                TokenType.Bool.ToString(),
                identifier.Value,
                new IdentifierNode(expressionTokens[0].Value)
            );
        }
        else if (expressionTokens.Count > 1)
        {
            ExpressionSequenceNode xsn = new();

            foreach (Token tk in expressionTokens)
            {
                if (tk.TokenType == TokenType.BoolLiteral)
                {
                    if (!bool.TryParse(tk.Value, out bool val))
                        throw new Exception($"{tk.Value} is not a valid bool literal");

                    xsn.Values.Add(new BoolLiteralNode(val));
                }
                else if (tk.TokenType == TokenType.Identifier)
                    xsn.Values.Add(new IdentifierNode(tk.Value));
                else if (tk.TokenType == TokenType.Not)
                    xsn.Values.Add(new NotNode());
                else
                    throw new Exception($"Unexpected token: {tk.TokenType}");
            }

            return new VariableDeclarationNode(
                TokenType.Bool.ToString(),
                identifier.Value,
                xsn
            );
        }
        else
            throw new Exception("Broken expression");
    }
}