// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text;

namespace CodeOfChaos.CliArgsParser.Generators.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IndentedStringBuilder : IGeneratorStringBuilder{
    private readonly StringBuilder _stringBuilder = new();
    private int _indent;
    public int IndentAmount {
        get => _indent;
        private set => _indent = value <= 0 ? 0 : value;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IGeneratorStringBuilder Append(string text) {
        _stringBuilder.AppendWithIndentation(IndentAmount, text);
        return this;
    }
    
    public IGeneratorStringBuilder AppendLine(string text) {
        _stringBuilder.AppendLineWithIndentation(IndentAmount, text);
        return this;
    }

    public IGeneratorStringBuilder UnIndent() {
        IndentAmount--;
        return this;
    }

    public IGeneratorStringBuilder Indent() {
        IndentAmount++;
        return this;
    }
    
    public IGeneratorStringBuilder UnIndentLine(string text) {
        _stringBuilder.AppendLineWithIndentation(--IndentAmount, text);
        return this;
    }
    
    public IGeneratorStringBuilder IndentLine() {
        _stringBuilder.AppendLineWithIndentation(++IndentAmount);
        return this;
    }
    
    public IGeneratorStringBuilder IndentLine(string text) {
        _stringBuilder.AppendLineWithIndentation(++IndentAmount, text);
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
        IndentAmount = 0;
        return this;
    }
}
