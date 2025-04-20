using System;

namespace ConvenientChests.Framework.ExceptionService;

/// <summary>
/// An exception to be raised when save data is malformed or fails to
/// correspond to the state of the game world.
/// </summary>
public class InvalidSaveDataException : Exception
{
    public InvalidSaveDataException(string message) : base(message)
    {
    }
}