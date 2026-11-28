using GXCompiler.GXLexer;

namespace GXCompiler.GXParser;

public partial class Parser
{
    private ExitNode ParseExitStmnt()
    {
        Expect(TokenType.Exit);
        Expect(TokenType.Semicolon);

        return new ExitNode();
    }
}