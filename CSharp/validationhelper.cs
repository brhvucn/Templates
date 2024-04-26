//A small helper class with a static method for validating. Requires FluentValidation
public class ValidationHelper
{
    public static ValidationResult Validate<T, K>(T input) where K : AbstractValidator<T>
    {

        var validatorInstance = (AbstractValidator<T>)Activator.CreateInstance(typeof(K));

        if (validatorInstance == null)
        {
            throw new InvalidOperationException($"Failed to create an instance of validator {typeof(K).Name}.");
        }

        var validationResult = validatorInstance.Validate(input);
        if (validationResult == null)
        {
            throw new ArgumentNullException(nameof(validationResult));
        }
        return validationResult;
    }
}
