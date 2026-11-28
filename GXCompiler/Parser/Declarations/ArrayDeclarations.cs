using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using GXCompiler.GXLexer;

namespace GXCompiler.GXParser;

public partial class Parser
{
    private VariableDeclarationNode ParseStrArrayDeclaration()
    {
        Advance();

        Token identifier = Expect(TokenType.Identifier);
        Expect(TokenType.Assign);
        Expect(TokenType.LeftBracket);

        List<Token> expressionTokens = [];

        while (Current.TokenType != TokenType.RightBracket && Current.TokenType != TokenType.EOF)
        {
            if (expressionTokens.Count == 0 ||
                expressionTokens.Last().TokenType == TokenType.Comma
            )
            {
                expressionTokens.Add(Expect(TokenType.Literal, TokenType.Identifier));
            }
            else
            {
                expressionTokens.Add(Expect(TokenType.Comma));
            }
        }

        Expect(TokenType.RightBracket);
        Expect(TokenType.Semicolon);

        if (expressionTokens.Last().TokenType == TokenType.Comma)
            throw new Exception("Expected RightBracket, got Comma");

        if (expressionTokens.Count == 0)
        {
            return new VariableDeclarationNode(
                TokenType.Str.ToString() + "[]",
                identifier.Value,
                new ArrayNode(TokenType.Str.ToString() + "[]", [])
            );
        }
        else if (expressionTokens.Count > 1)
        {
            List<ExpressionNode> nodes = [];

            foreach (Token tk in expressionTokens)
            {
                if (tk.TokenType == TokenType.Literal)
                    nodes.Add(new LiteralNode(tk.Value));
                else if (tk.TokenType == TokenType.Identifier)
                    nodes.Add(new IdentifierNode(tk.Value));
                else if (tk.TokenType == TokenType.Comma)
                    continue;
                else
                    throw new Exception($"Unexpected token: {tk.TokenType}");
            }

            ArrayNode arn = new(TokenType.Str.ToString() + "[]", [.. nodes]);

            return new VariableDeclarationNode(
                TokenType.Str.ToString() + "[]",
                identifier.Value,
                arn
            );
        }
        else
            throw new Exception("Broken expression");
    }

    private VariableDeclarationNode ParseNumArrayDeclaration(TokenType btk)
    {
        Advance();

        Token identifier = Expect(TokenType.Identifier);
        Expect(TokenType.Assign);
        Expect(TokenType.LeftBracket);

        List<Token> expressionTokens = [];

        while (Current.TokenType != TokenType.RightBracket && Current.TokenType != TokenType.EOF)
        {
            if (expressionTokens.Count == 0 ||
                expressionTokens.Last().TokenType == TokenType.Comma
            )
            {
                expressionTokens.Add(Expect(TokenType.Number, TokenType.Identifier));
            }
            else
            {
                expressionTokens.Add(Expect(TokenType.Comma));
            }
        }

        Expect(TokenType.RightBracket);
        Expect(TokenType.Semicolon);

        if (expressionTokens.Last().TokenType == TokenType.Comma)
            throw new Exception("Expected RightBracket, got Comma");

        if (expressionTokens.Count == 0)
        {
            return new VariableDeclarationNode(
                btk.ToString() + "[]",
                identifier.Value,
                new ArrayNode(btk.ToString() + "[]", [])
            );
        }
        else if (expressionTokens.Count > 1)
        {
            List<ExpressionNode> nodes = [];

            foreach (Token tk in expressionTokens)
            {
                if (tk.TokenType == TokenType.Number)
                    nodes.Add(new NumberNode(tk.Value));
                else if (tk.TokenType == TokenType.Identifier)
                    nodes.Add(new IdentifierNode(tk.Value));
                else if (tk.TokenType == TokenType.Comma)
                    continue;
                else
                    throw new Exception($"Unexpected token: {tk.TokenType}");
            }

            ArrayNode arn = new(btk.ToString() + "[]", [.. nodes]);

            return new VariableDeclarationNode(
                btk.ToString() + "[]",
                identifier.Value,
                arn
            );
        }
        else
            throw new Exception("Broken expression");
    }

    private VariableDeclarationNode ParseBoolArrayDeclaration()
    {
        Advance();

        Token identifier = Expect(TokenType.Identifier);
        Expect(TokenType.Assign);
        Expect(TokenType.LeftBracket);

        List<Token> expressionTokens = [];

        while (Current.TokenType != TokenType.RightBracket && Current.TokenType != TokenType.EOF)
        {
            if (expressionTokens.Count == 0 ||
                expressionTokens.Last().TokenType == TokenType.Comma
            )
            {
                expressionTokens.Add(Expect(TokenType.BoolLiteral, TokenType.Identifier));
            }
            else
            {
                expressionTokens.Add(Expect(TokenType.Comma));
            }
        }

        Expect(TokenType.RightBracket);
        Expect(TokenType.Semicolon);

        if (expressionTokens.Last().TokenType == TokenType.Comma)
            throw new Exception("Expected RightBracket, got Comma");

        if (expressionTokens.Count == 0)
        {
            return new VariableDeclarationNode(
                TokenType.Bool.ToString() + "[]",
                identifier.Value,
                new ArrayNode(TokenType.Bool.ToString() + "[]", [])
            );
        }
        else if (expressionTokens.Count > 1)
        {
            List<ExpressionNode> nodes = [];

            foreach (Token tk in expressionTokens)
            {
                bool v = bool.TryParse(tk.Value, out bool val);

                if (tk.TokenType == TokenType.BoolLiteral)
                    nodes.Add(new BoolLiteralNode(val));
                else if (tk.TokenType == TokenType.Identifier)
                    nodes.Add(new IdentifierNode(tk.Value));
                else if (tk.TokenType == TokenType.Comma)
                    continue;
                else
                    throw new Exception($"Unexpected token: {tk.TokenType}");
            }

            ArrayNode arn = new(TokenType.Bool.ToString() + "[]", [.. nodes]);

            return new VariableDeclarationNode(
                TokenType.Bool.ToString() + "[]",
                identifier.Value,
                arn
            );
        }
        else
            throw new Exception("Broken expression");
    }
}