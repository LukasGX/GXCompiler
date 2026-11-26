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
        ["str"] = TokenType.Str,
        ["int"] = TokenType.Int,
        ["dec"] = TokenType.Dec,
        ["bool"] = TokenType.Bool,
        ["rex"] = TokenType.Rex,
        ["true"] = TokenType.BoolLiteral,
        ["false"] = TokenType.BoolLiteral,
        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["switch"] = TokenType.Switch,
        ["repeat"] = TokenType.Repeat,
        ["iterate"] = TokenType.Iterate,
        ["while"] = TokenType.While,
        ["class"] = TokenType.Class,
        ["init"] = TokenType.Init,
        ["entrypoint"] = TokenType.Entrypoint,
        ["method"] = TokenType.Method,
        ["private"] = TokenType.Private,
        ["return"] = TokenType.Return,
        ["inst"] = TokenType.Inst,
        ["case"] = TokenType.Case,
        ["new"] = TokenType.New,
        ["using"] = TokenType.Using,
        ["try"] = TokenType.Try,
        ["treat"] = TokenType.Treat,
        ["afterwards"] = TokenType.Afterwards,
        ["ns"] = TokenType.Ns,
        ["default"] = TokenType.Default,
        ["continue"] = TokenType.Continue,
        ["break"] = TokenType.Break
    };

    public List<Token> Lex(string source)
    {
        List<Token> tokens = [];

        int line = 1;
        int column = 0;

        int i = 0;

        while (i < source.Length)
        {
            char c = source[i];

            if (c == '\n')
            {
                line++;
                column = 0;
                i++;
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                i++;
                column++;
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

                tokens.Add(new Token(
                    TokenType.Number,
                    number,
                    line,
                    column
                ));
                column += number.Length;
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
                        value,
                        line,
                        column
                    ));
                else
                    tokens.Add(new Token(
                        TokenType.Identifier,
                        value,
                        line,
                        column
                    ));
                column += value.Length;
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

                tokens.Add(new Token(
                    TokenType.Literal,
                    value,
                    line,
                    column
                ));
                column += value.Length + 2;
                continue;
            }

            if (c == '/')
            {
                string value = "";

                if (i < source.Length - 1 && source[i+1] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.DivideAssign,
                        "/=",
                        line,
                        column
                    ));
                    i += 2;
                    column += 2;
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
                            value,
                            line,
                            column
                        ));
                        column += value.Length + 2;
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
                            value,
                            line,
                            column
                        ));
                        column += value.Length + 1;
                        continue;
                    }
                }
                else
                {
                    tokens.Add(new Token(
                        TokenType.Divide,
                        "/",
                        line,
                        column
                    ));
                    column++;
                    continue;
                }
            }

            if (c == '+' || c == '-' || c == '*' || c == '^' || c == '<' || c == '>')
            {
                if (i < source.Length - 1 && source[i+1] == '=')
                {
                    if (c == '+')
                        tokens.Add(new Token(
                            TokenType.PlusAssign,
                            "+=",
                            line,
                            column
                        ));
                    else if (c == '-')
                        tokens.Add(new Token(
                            TokenType.MinusAssign,
                            "-=",
                            line,
                            column
                        ));
                    else if (c == '*')
                        tokens.Add(new Token(
                            TokenType.MultiplyAssign,
                            "*=",
                            line,
                            column
                        ));
                    else if (c == '^')
                        tokens.Add(new Token(
                            TokenType.PowerAssign,
                            "^=",
                            line,
                            column
                        ));
                    else if (c == '<')
                        tokens.Add(new Token(
                            TokenType.LowerOrEqual,
                            "<=",
                            line,
                            column
                        ));
                    else if (c == '>')
                        tokens.Add(new Token(
                            TokenType.GreaterOrEqual,
                            ">=",
                            line,
                            column
                        ));
                    i += 2;
                    column += 2;
                    continue;
                }
                else
                {
                    if (c == '+')
                        tokens.Add(new Token(
                            TokenType.Plus,
                            "+",
                            line,
                            column
                        ));
                    else if (c == '-')
                        tokens.Add(new Token(
                            TokenType.Minus,
                            "-",
                            line,
                            column
                        ));
                    else if (c == '*')
                        tokens.Add(new Token(
                            TokenType.Multiply,
                            "*",
                            line,
                            column
                        ));
                    else if (c == '^')
                        tokens.Add(new Token(
                            TokenType.Power,
                            "^",
                            line,
                            column
                        ));
                    else if (c == '<')
                        tokens.Add(new Token(
                            TokenType.Lower,
                            "<",
                            line,
                            column
                        ));
                    else if (c == '>')
                        tokens.Add(new Token(
                            TokenType.Greater,
                            ">",
                            line,
                            column
                        ));
                    i++;
                    column++;
                    continue;
                }
            }

            if (c == '=')
            {
                if (i < source.Length - 2 && source[i+1] == '=' && source[i+2] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.StrictEqual,
                        "===",
                        line,
                        column
                    ));
                    i += 3;
                    column += 3;
                    continue;
                }
                else if (i < source.Length - 1 && source[i+1] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.Equal,
                        "==",
                        line,
                        column
                    ));
                    i += 2;
                    column += 2;
                    continue;
                }
                else
                {
                    tokens.Add(new Token(
                        TokenType.Assign,
                        "=",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    continue;
                }
            }

            if (c == '!')
            {
                if (i < source.Length - 1 && source[i+1] == '=')
                {
                    tokens.Add(new Token(
                        TokenType.NotEqual,
                        "!=",
                        line,
                        column
                    ));
                    i += 2;
                    column += 2;
                    continue;
                }
                else
                {
                    tokens.Add(new Token(
                        TokenType.Not,
                        "!",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    continue;
                }
            }

            switch (c)
            {
                case ';':
                    tokens.Add(new Token(
                        TokenType.Semicolon,
                        ";",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case ',':
                    tokens.Add(new Token(
                        TokenType.Comma,
                        ",",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case '.':
                    tokens.Add(new Token(
                        TokenType.Dot,
                        ".",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case ':':
                    tokens.Add(new Token(
                        TokenType.Colon,
                        ":",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case '#':
                    tokens.Add(new Token(
                        TokenType.Hashtag,
                        "#",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case '(':
                    tokens.Add(new Token(
                        TokenType.LeftParen,
                        "(",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case ')':
                    tokens.Add(new Token(
                        TokenType.RightParen,
                        ")",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case '{':
                    tokens.Add(new Token(
                        TokenType.LeftBrace,
                        "{",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case '}':
                    tokens.Add(new Token(
                        TokenType.RightBrace,
                        "}",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case '[':
                    tokens.Add(new Token(
                        TokenType.LeftBracket,
                        "[",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                case ']':
                    tokens.Add(new Token(
                        TokenType.RightBracket,
                        "]",
                        line,
                        column
                    ));
                    i++;
                    column++;
                    break;

                default:
                    throw new Exception($"Unknown character: {c}");
            }
        }

        tokens.Add(new Token(
            TokenType.EOF,
            "",
            line,
            column
        ));

        return tokens;
    }
}