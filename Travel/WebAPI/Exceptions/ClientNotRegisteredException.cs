namespace WebAPI.Exceptions;

public class ClientNotRegisteredException(string? message) : Exception(message);