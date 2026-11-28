using GXCompiler.GXLexer;

namespace GXCompiler.GXParser;

public partial class Parser
{
    private OutNode ParseOutStmnt()
    {
        Advance();

        if (Current.TokenType == TokenType.Literal)
        {
            Token value = Expect(TokenType.Literal);
            Expect(TokenType.Semicolon);

            return new OutNode
            {
                Value = new LiteralNode(value.Value)
            };
        }
        else if (Current.TokenType == TokenType.Identifier)
        {
            Token value = Expect(TokenType.Identifier);
            Expect(TokenType.Semicolon);

            return new OutNode
            {
                Value = new IdentifierNode(value.Value)
            };
        }
        else
            throw new Exception($"Unexpected token: {Current.TokenType}");
    }

    private ShoutNode ParseShoutStmnt()
    {
        Advance();

        if (Current.TokenType == TokenType.Literal)
        {
            Token value = Expect(TokenType.Literal);
            Expect(TokenType.Semicolon);

            return new ShoutNode
            {
                Value = new LiteralNode(value.Value)
            };
        }
        else if (Current.TokenType == TokenType.Identifier)
        {
            Token value = Expect(TokenType.Identifier);
            Expect(TokenType.Semicolon);

            return new ShoutNode
            {
                Value = new IdentifierNode(value.Value)
            };
        }
        else
            throw new Exception($"Unexpected token: {Current.TokenType}");
    }
}