using System.ComponentModel.Design;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using GXCompiler.GXLexer;

namespace GXCompiler.GXParser;

public partial class Parser(List<Token> tks)
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
            throw new Exception($"Expected {firstType} or {string.Join(" or ", otherTypes)}, got {Current.TokenType}");

        return Advance();
    }

    public ProgramNode Parse()
    {
        ProgramNode program = new();

        while (Current.TokenType != TokenType.EOF)
        {
            switch (Current.TokenType)
            {
                case TokenType.Comment:
                    break;

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
            StatementNode statement = ParseStatement();

            if (statement is CommentNode)
                continue;
            else
                block.Statements.Add(statement);
        }

        return block;
    }

    private StatementNode ParseStatement()
    {
        TokenType t = Current.TokenType;

        if (t == TokenType.Comment)
            return new CommentNode();
        else if (t == TokenType.Out)
            return ParseOutStmnt();
        else if (t == TokenType.Shout)
            return ParseShoutStmnt();
        else if (t == TokenType.Exit)
            return ParseExitStmnt();
        else if (t == TokenType.Str &&
                tokens[position + 1].TokenType == TokenType.LeftBracket &&
                tokens[position + 2].TokenType == TokenType.RightBracket)
        {
            Expect(TokenType.Str); Expect(TokenType.LeftBracket);
            return ParseStrArrayDeclaration();
        }
        else if (t == TokenType.Str)
            return ParseStrDeclaration();
        else if ((t == TokenType.Int || t == TokenType.Dec) &&
                tokens[position + 1].TokenType == TokenType.LeftBracket &&
                tokens[position + 2].TokenType == TokenType.RightBracket)
        {
            Expect(TokenType.Int, TokenType.Dec); Expect(TokenType.LeftBracket);
            return ParseNumArrayDeclaration(t);
        }
        else if (t == TokenType.Int || t == TokenType.Dec)
            return ParseNumDeclaration();
        else if (t == TokenType.Bool &&
                tokens[position + 1].TokenType == TokenType.LeftBracket &&
                tokens[position + 2].TokenType == TokenType.RightBracket)
        {
            Expect(TokenType.Bool); Expect(TokenType.LeftBracket);
            return ParseBoolArrayDeclaration();
        }
        else if (t == TokenType.Bool)
            return ParseBoolDeclaration();

        throw new Exception($"Unexpected token: {Current.TokenType}");
    }
}