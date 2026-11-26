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

                int dots = 0;

                while (i < source.Length && (char.IsDigit(source[i]) || source[i] == '.'))
                {
                    if (source[i] == '.')
                        dots++;

                    if (dots > 1)
                        throw new Exception("Unknown character .");

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

                if (i < source.Length - 1 && source[i+1] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.DivideAssign,
                        "/="
                    ));
                    i += 2;
                    continue;
                }
                else if (i < source.Length - 1 && source[i+1] == '/')
                {
                    i++;

                    if (i < source.Length - 1 && source[i+1] == '/')
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
                else
                {
                    tokens.Add(new Token(
                        TokenType.Divide,
                        "/"
                    ));
                    continue;
                }
            }

            if (c == '+' || c == '-' || c == '*' || c == '^')
            {
                if (i < source.Length - 1 && source[i+1] == '=')
                {
                    if (c == '+')
                        tokens.Add(new Token(
                            TokenType.PlusAssign,
                            "+="
                        ));
                    else if (c == '-')
                        tokens.Add(new Token(
                            TokenType.MinusAssign,
                            "-="
                        ));
                    else if (c == '*')
                        tokens.Add(new Token(
                            TokenType.MultiplyAssign,
                            "*="
                        ));
                    else if (c == '^')
                        tokens.Add(new Token(
                            TokenType.PowerAssign,
                            "^="
                        ));
                    i += 2;
                    continue;
                }
                else
                {
                    if (c == '+')
                        tokens.Add(new Token(
                            TokenType.Plus,
                            "+"
                        ));
                    else if (c == '-')
                        tokens.Add(new Token(
                            TokenType.Minus,
                            "-"
                        ));
                    else if (c == '*')
                        tokens.Add(new Token(
                            TokenType.Multiply,
                            "*"
                        ));
                    else if (c == '^')
                        tokens.Add(new Token(
                            TokenType.Power,
                            "^"
                        ));
                    i++;
                    continue;
                }
            }

            if (c == '=')
            {
                if (i < source.Length - 2 && source[i+1] == '=' && source[i+2] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.StrictEqual,
                        "==="
                    ));
                    i += 3;
                    continue;
                }
                else if (i < source.Length - 1 && source[i+1] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.Equal,
                        "=="
                    ));
                    i += 2;
                    continue;
                }
                else
                {
                    tokens.Add(new Token(
                        TokenType.Assign,
                        "="
                    ));
                    i++;
                    continue;
                }
            }

            if (c == '!')
            {
                if (i < source.Length - 1 && source[i+1] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.NotEqual,
                        "!="
                    ));
                    i += 2;
                    continue;
                }
                else
                {
                    tokens.Add(new Token(
                        TokenType.Not,
                        "!"
                    ));
                    i++;
                    continue;
                }
            }

            switch (c)
            {
                case ';':
                    tokens.Add(new Token(
                        TokenType.Semicolon,
                        ";"
                    ));
                    i++;
                    break;

                case ',':
                    tokens.Add(new Token(
                        TokenType.Comma,
                        ","
                    ));
                    i++;
                    break;

                case '.':
                    tokens.Add(new Token(
                        TokenType.Dot,
                        "."
                    ));
                    i++;
                    break;

                case ':':
                    tokens.Add(new Token(
                        TokenType.Colon,
                        ":"
                    ));
                    i++;
                    break;

                case '(':
                    tokens.Add(new Token(
                        TokenType.LeftParen,
                        "("
                    ));
                    i++;
                    break;

                case ')':
                    tokens.Add(new Token(
                        TokenType.RightParen,
                        ")"
                    ));
                    i++;
                    break;

                case '{':
                    tokens.Add(new Token(
                        TokenType.LeftBrace,
                        "{"
                    ));
                    i++;
                    break;

                case '}':
                    tokens.Add(new Token(
                        TokenType.RightBrace,
                        "}"
                    ));
                    i++;
                    break;

                case '[':
                    tokens.Add(new Token(
                        TokenType.LeftBracket,
                        "["
                    ));
                    i++;
                    break;

                case ']':
                    tokens.Add(new Token(
                        TokenType.RightBracket,
                        "]"
                    ));
                    i++;
                    break;

                default:
                    throw new Exception($"Unknown character: {c}");
            }
        }

        return tokens;
    }
}