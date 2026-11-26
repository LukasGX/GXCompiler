using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using GXCompiler.GXLexer;

namespace GXCompiler.GXParser;

public class Parser(List<Token> tks)
{
    private readonly List<Token> tokens = tks;
    private int position = 0;

    private Token Current => tokens[position];

    private Token Advance()
    {
        Token token = Current;
        position++;
        return token;
    }

    private Token Expect(TokenType type)
    {
        if (Current.TokenType != type)
            throw new Exception($"Expected {type}, got {Current.TokenType}");

        return Advance();
    }

    private Token Expect(TokenType firstType, params TokenType[] otherTypes)
    {
        if (Current.TokenType != firstType && !otherTypes.Contains(Current.TokenType))
            throw new Exception($"Expected {firstType} {string.Join("or", otherTypes)}, got {Current.TokenType}");

        return Advance();
    }

    public ProgramNode Parse()
    {
        ProgramNode program = new();

        while (Current.TokenType != TokenType.EOF)
        {
            switch (Current.TokenType)
            {
                case TokenType.Entrypoint:
                    program.Entrypoint = ParseEntrypoint();
                    break;

                // case TokenType.Class:
                //     program.Classes.Add(ParseClass());
                //     break;

                // case TokenType.Hashtag:
                //     ParseDirective();
                //     break;

                default:
                    throw new Exception(
                        $"Unerwartetes Token: {Current.TokenType}"
                    );
            }
        }

        return program;
    }

    private EntrypointNode ParseEntrypoint()
    {
        Expect(TokenType.Entrypoint);
        Expect(TokenType.LeftParen);
        Expect(TokenType.RightParen);
        Expect(TokenType.LeftBrace);

        BlockNode body = ParseBlock();

        Expect(TokenType.RightBrace);

        return new EntrypointNode
        {
            Body = body
        };
    }

    private BlockNode ParseBlock()
    {
        BlockNode block = new();

        while (Current.TokenType != TokenType.RightBrace &&
               Current.TokenType != TokenType.EOF)
        {
            block.Statements.Add(ParseStatement());
        }

        return block;
    }

    private StatementNode ParseStatement()
    {
        if (Current.TokenType == TokenType.Out)
        {
            Advance();

            if (Current.TokenType == TokenType.Literal)
            {
                Token value = Expect(TokenType.Literal);
                Expect(TokenType.Semicolon);

                return new OutNode
                {
                    Value = new LiteralNode
                    {
                        Value = value.Value
                    }
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
        }

        throw new Exception($"Unexpected token: {Current.TokenType}");
    }
}