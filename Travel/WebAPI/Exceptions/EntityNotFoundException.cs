namespace WebAPI.Exceptions;

public class EntityNotFoundException(string? message) : Exception(message);