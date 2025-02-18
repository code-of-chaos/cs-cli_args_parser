﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ICommand<in T> :
    // Ah blessed be the workarounds.
    INonGenericCommandInterfaces
    where T : struct, IParameters {
    Task ExecuteAsync(T parameters);
}
