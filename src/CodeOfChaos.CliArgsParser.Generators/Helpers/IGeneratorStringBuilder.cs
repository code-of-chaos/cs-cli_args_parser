// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser.Generators.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IGeneratorStringBuilder {
    public IGeneratorStringBuilder Append(string text) ;
    public IGeneratorStringBuilder AppendLine(string text) ;
    public IGeneratorStringBuilder UnIndent() ;
    public IGeneratorStringBuilder Indent() ;
    public IGeneratorStringBuilder UnIndentLine(string text) ;
    public IGeneratorStringBuilder IndentLine() ;
    public IGeneratorStringBuilder IndentLine(string text) ;
    public string ToString();
    public string ToStringAndClear() ;
}
