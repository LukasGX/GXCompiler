using GXCompiler.GXLexer;

namespace GXCompiler;

public static class Program {
	public static void Main(string[] args) {
        string content = File.ReadAllText("program.gxc");

		Lexer lexer = new();

		List<Token> tokens = lexer.Lex(content);
		foreach (Token t in tokens)
		{
			Console.WriteLine($"{t.TokenType} / {t.Value} / {t.Line} / {t.Column}");
		}
		Console.WriteLine($"Lexed in {tokens.Count} tokens.");
	}
}
