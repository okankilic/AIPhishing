namespace AIPhishing.Common.Exceptions;

public class BusinessException(string message)
    : Exception(message)
{
    public static BusinessException Required(string propertyName)
    {
        return new BusinessException($"{propertyName} is required");
    }
    
    public static BusinessException Invalid(string propertyName)
    {
        return new BusinessException($"{propertyName} is invalid");
    }
    
    public static BusinessException NotFound(string propertyName, Guid id)
    {
        return new BusinessException($"{propertyName} not found. Id: {id}");
    }
    
    public static BusinessException NotFound(string propertyName, string message)
    {
        return new BusinessException($"{propertyName} not found. {message}");
    }
    
    public static BusinessException InUse(string propertyName, string value)
    {
        return new BusinessException($"{propertyName}: {value} is already in use.");
    }
}