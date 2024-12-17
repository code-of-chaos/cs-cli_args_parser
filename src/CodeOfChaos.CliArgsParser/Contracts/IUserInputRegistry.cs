// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser.Contracts;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUserInputRegistry : IDisposable {
    void IngestString(IEnumerable<string> input);
    void IngestString(string[] input);
    void IngestString(string input);

    T GetParameterByPossibleNames<T>(string name, string shortName);
    T? GetOptionalParameterByPossibleNames<T>(string name, string shortName);

    T GetParameter<T>(string key);
    T? GetOptionalParameter<T>(string key);
}
