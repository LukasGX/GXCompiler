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
    Equals,
    Semicolon,
    EOF
}

public class Token(TokenType tokenType, string value)
{
    public TokenType TokenType = tokenType;
    public string Value = value;
}