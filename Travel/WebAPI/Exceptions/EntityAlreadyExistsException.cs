namespace WebAPI.Exceptions;

public class EntityAlreadyExistsException(string? message) : Exception(message);