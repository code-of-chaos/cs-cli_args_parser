// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text;

namespace CodeOfChaos.CliArgsParser.Generators.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IndentedStringBuilder {
    private readonly StringBuilder _stringBuilder = new();
    private int _indent;
    public int Indent {
        get => _indent;
        private set => _indent = value <= 0 ? 0 : value;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IndentedStringBuilder Append(string text) {
        _stringBuilder.AppendWithIndentation(Indent, text);
        return this;
    }
    
    public IndentedStringBuilder AppendLine(string text) {
        _stringBuilder.AppendLineWithIndentation(Indent, text);
        return this;
    }

    public IndentedStringBuilder UnIndent() {
        Indent--;
        return this;
    }
    public IndentedStringBuilder UnIndentLine(string text) {
        _stringBuilder.AppendLineWithIndentation(--Indent, text);
        return this;
    }
    
    public IndentedStringBuilder IndentLine() {
        _stringBuilder.AppendLineWithIndentation(++Indent);
        return this;
    }
    
    public IndentedStringBuilder IndentLine(string text) {
        _stringBuilder.AppendLineWithIndentation(++Indent, text);
        return this;
    }
    
    public override string ToString() => _stringBuilder.ToString();
    public string ToStringAndClear() {
        string result = ToString();
        Clear();
        return result;
    }

    public IndentedStringBuilder Clear() {
        _stringBuilder.Clear();
        Indent = 0;
        return this;
    }
}
