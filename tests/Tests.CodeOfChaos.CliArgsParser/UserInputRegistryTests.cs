// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using JetBrains.Annotations;

namespace Tests.CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestSubject(typeof(UserInputRegistry))]
public class UserInputRegistryTests {
    [Test]
    public async Task Test_IngestString_ParsesSingleKeyValuePair() {
        // Arrange
        var registry = new UserInputRegistry();
        string input = "--key=value";

        // Act
        registry.IngestString(input);

        // Assert
        await Assert.That(registry.GetParameter<string>("--key")).IsNotNull().Because("The parameter should exist");
    }

    [Test]
    public async Task Test_IngestString_ParsesMultipleKeyValuePairs() {
        // Arrange
        var registry = new UserInputRegistry();
        string input = "--key1=value1 --key2=value2";

        // Act
        registry.IngestString(input);
        string keyValue1 = registry.GetParameter<string>("--key1");
        string keyValue2 = registry.GetParameter<string>("--key2");

        // Assert
        await Assert.That(keyValue1).IsNotNull().Because("The parameter should exist");
        await Assert.That(keyValue2).IsNotNull().Because("The parameter should exist");
        await Assert.That(keyValue1).IsEqualTo("value1");
        await Assert.That(keyValue2).IsEqualTo("value2");
    }

    [Test]
    [Arguments("--flag")]
    // [Arguments( "-f" )]
    public async Task Test_IngestString_ParsesFlags(string input) {
        // Arrange
        var registry = new UserInputRegistry();

        // Act
        registry.IngestString(input);
        bool flag = registry.GetParameter<bool>("--flag");

        // Assert
        await Assert.That(flag).IsNotNull().Because("The parameter should exist");
        await Assert.That(flag).IsEqualTo(true);
    }

    [Test]
    public async Task Test_IngestString_ParsesQuotedString() {
        // Arrange
        var registry = new UserInputRegistry();
        string input = "\"This is a test string\"";

        // Act
        registry.IngestString(input);

        // Assert
        await Assert.That(registry.GetParameter<string>("quotedString_0")).IsNotNull().Because("The parameter should exist");
    }

    [Test]
    public async Task Test_IngestString_ParsesPositionalArguments() {
        // Arrange
        var registry = new UserInputRegistry();
        const string input = "arg1 arg2";

        // Act
        registry.IngestString(input);
        string positional0 = registry.GetParameter<string>("positional_0");
        string positional1 = registry.GetParameter<string>("positional_1");

        // Assert
        await Assert.That(positional0).IsNotNull();
        await Assert.That(positional1).IsNotNull();
        await Assert.That(positional0).IsEqualTo("arg1");
        await Assert.That(positional1).IsEqualTo("arg2");
    }

    [Test]
    public async Task Test_GetParameter_ThrowsKeyNotFoundException_WhenParameterNotFound() {
        // Arrange
        var registry = new UserInputRegistry();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Task.FromResult(registry.GetParameter<string>("nonexistent")));
    }

    [Test]
    public async Task Test_GetOptionalParameter_ReturnsNull_WhenParameterNotFound() {
        // Arrange
        var registry = new UserInputRegistry();

        // Act
        string? result = registry.GetOptionalParameter<string>("nonexistent");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Test_GetOptionalParameter_ReturnsValue_WhenParameterExists() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("--key=value");

        // Act
        string? result = registry.GetOptionalParameter<string>("--key");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_Dispose_ClearsParameters() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("--key=value");

        // Act
        registry.Dispose();

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Task.FromResult(registry.GetParameter<string>("key")));
    }


    [Test]
    public async Task Test_IngestString_ParsesComplexInputWithMixedArguments() {
        // Arrange
        var registry = new UserInputRegistry();
        string input = "--key1=value1 --key2=value2 -f --quoted=\"This is a test\" arg1 arg2 --boolFlag=true --negativeFlag=false";

        // Act
        registry.IngestString(input);
        string key1 = registry.GetParameter<string>("--key1");
        string key2 = registry.GetParameter<string>("--key2");
        bool shortFlag = registry.GetParameter<bool>("-f");
        string quotedString = registry.GetParameter<string>("--quoted");
        string positional0 = registry.GetParameter<string>("positional_0");
        string positional1 = registry.GetParameter<string>("positional_1");
        bool boolFlag = registry.GetParameter<bool>("--boolFlag");
        bool negativeFlag = registry.GetParameter<bool>("--negativeFlag");

        // Assert
        await Assert.That(key1).IsNotNull().Because("The parameter 'key1' should exist");
        await Assert.That(key1).IsEqualTo("value1");

        await Assert.That(key2).IsNotNull().Because("The parameter 'key2' should exist");
        await Assert.That(key2).IsEqualTo("value2");

        await Assert.That(shortFlag).IsNotNull().Because("The flag '-f' should be a valid boolean value");
        await Assert.That(shortFlag).IsEqualTo(true);

        await Assert.That(quotedString).IsNotNull().Because("The quoted string should be parsed correctly");
        await Assert.That(quotedString).IsEqualTo("This is a test");

        await Assert.That(positional0).IsNotNull().Because("The first positional argument should exist");
        await Assert.That(positional0).IsEqualTo("arg1");

        await Assert.That(positional1).IsNotNull().Because("The second positional argument should exist");
        await Assert.That(positional1).IsEqualTo("arg2");

        await Assert.That(boolFlag).IsNotNull().Because("The boolean flag 'boolFlag' should exist");
        await Assert.That(boolFlag).IsEqualTo(true);

        await Assert.That(negativeFlag).IsNotNull().Because("The boolean flag 'negativeFlag' should exist");
        await Assert.That(negativeFlag).IsEqualTo(false);
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_ReturnsValue_WhenFullNameExists() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("--fullName=value");

        // Act
        string result = registry.GetParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_ReturnsValue_WhenShortNameExists() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("-f=value");

        // Act
        string result = registry.GetParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_ThrowsKeyNotFoundException_WhenParameterNotFound() {
        // Arrange
        var registry = new UserInputRegistry();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Task.FromResult(registry.GetParameterByPossibleNames<string>("--nonexistent", "-n")));
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_ReturnsValue_WhenFullNameExists() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("--fullName=value");

        // Act
        string? result = registry.GetOptionalParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_ReturnsValue_WhenShortNameExists() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("-f=value");

        // Act
        string? result = registry.GetOptionalParameterByPossibleNames<string>("--fullName", "-f");

        // Assert
        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_ReturnsNull_WhenParameterNotFound() {
        // Arrange
        var registry = new UserInputRegistry();

        // Act
        string? result = registry.GetOptionalParameterByPossibleNames<string>("--nonexistent", "-n");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Test_GetParameterByPossibleNames_BooleanFlag_ReturnsValue() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("--flag");

        // Act
        bool flag = registry.GetParameterByPossibleNames<bool>("--flag", "-f");

        // Assert
        await Assert.That(flag).IsEqualTo(true);
    }

    [Test]
    public async Task Test_GetOptionalParameterByPossibleNames_BooleanFlag_ReturnsValue() {
        // Arrange
        var registry = new UserInputRegistry();
        registry.IngestString("--flag");

        // Act
        bool? flag = registry.GetOptionalParameterByPossibleNames<bool>("--flag", "-f");

        // Assert
        await Assert.That(flag).IsEqualTo(true);
    }
}
