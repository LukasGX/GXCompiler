namespace GXCompiler.GXLexer;

public enum TokenType
{
    Comment,
    Out,
    Shout,
    Exit,
    Literal,
    Int,
    Number,
    Identifier,
    Plus,
    Minus,
    Multiply,
    Divide,
    Power,
    Equal,
    StrictEqual,
    NotEqual,
    Not,
    Assign,
    PlusAssign,
    MinusAssign,
    MultiplyAssign,
    DivideAssign,
    PowerAssign,
    Semicolon,
    Comma,
    Dot,
    Colon,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    EOF
}

public class Token(TokenType tokenType, string value)
{
    public TokenType TokenType = tokenType;
    public string Value = value;
}