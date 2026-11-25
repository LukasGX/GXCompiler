using System.Diagnostics;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;

namespace GXCompiler.GXLexer;

public class Lexer()
{
    readonly Dictionary<string, TokenType> Keywords = new()
    {
        ["out"] = TokenType.Out,
        ["shout"] = TokenType.Shout,
        ["exit"] = TokenType.Exit,
        ["int"] = TokenType.Int
    };

    public List<Token> Lex(string source)
    {
        List<Token> tokens = [];

        int i = 0;

        while (i < source.Length)
        {
            char c = source[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c))
            {
                string number = "";

                while (i < source.Length && char.IsDigit(source[i]))
                {
                    number += source[i];
                    i++;
                }

                tokens.Add(new Token(TokenType.Number, number));
                continue;
            }

            if (char.IsLetter(c))
            {
                string value = "";

                while (i < source.Length && (char.IsLetter(source[i]) || char.IsDigit(source[i]) || source[i] == '_'))
                {
                    value += source[i];
                    i++;
                }

                if (Keywords.TryGetValue(value, out TokenType type))
                    tokens.Add(new Token(
                        type,
                        value
                    ));
                else
                    tokens.Add(new Token(
                        TokenType.Identifier,
                        value
                    ));
                continue;
            }

            if (c == '"')
            {
                string value = "";
                int backslashCount = 0;
                bool closed = false;

                i++;

                while (i < source.Length)
                {
                    char current = source[i];

                    if (current == '"' && backslashCount % 2 == 0)
                    {
                        closed = true;
                        i++;
                        break;
                    }

                    value += current;

                    if (current == '\\')
                        backslashCount++;
                    else
                        backslashCount = 0;

                    i++;
                }

                if (!closed)
                    throw new Exception("Unterminated string literal");

                tokens.Add(new Token(TokenType.Literal, value));
                continue;
            }

            if (c == '/')
            {
                string value = "";

                i++;

                if (i >= source.Length || source[i] != '/')
                    throw new Exception($"Unknown character: {c}");

                i++;

                if (i < source.Length && source[i] == '/')
                {
                    i++;

                    int slashes = 0;
                    bool closed = false;

                    while (i < source.Length)
                    {
                        if (source[i] == '/') slashes++;
                        else slashes = 0;

                        if (slashes >= 3)
                        {
                            closed = true;
                            i++;
                            break;
                        }

                        value += source[i];
                        i++;
                    }

                    if (!closed)
                        throw new Exception("Unterminated multi-line comment");

                    value = value[..^2];

                    tokens.Add(new Token(
                        TokenType.Comment,
                        value
                    ));
                    continue;
                }
                else
                {
                    while (i < source.Length && source[i] != '\n')
                    {
                        value += source[i];
                        i++;
                    }

                    tokens.Add(new Token(
                        TokenType.Comment,
                        value
                    ));
                    continue;
                }
            }

            switch (c)
            {
                case '+':
                    tokens.Add(new Token(TokenType.Plus, "+"));
                    i++;
                    break;

                case '=':
                    tokens.Add(new Token(TokenType.Equals, "="));
                    i++;
                    break;

                case ';':
                    tokens.Add(new Token(TokenType.Semicolon, ";"));
                    i++;
                    break;

                default:
                    throw new Exception($"Unknown character: {c}");
            }
        }

        return tokens;
    }
}