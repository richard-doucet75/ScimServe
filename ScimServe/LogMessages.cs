namespace ScimServe;

public static class LogMessages
{
    public const string ExecutingUseCase = "Executing {UseCase} use case";
    public const string PermissionDenied = "Permission denied for {Operation} operation";
    public const string ExceptionOccurred = "Exception occurred in {Operation} operation: {Exception}";
    public const string AttemptingUsernameReservation = "Attempting to reserve username: {UserName}";
    public const string UsernameConflict = "Username conflict or reservation failed for: {UserName}";
    public const string HashingPassword = "Hashing password for user: {UserName}";
    public const string CreatingUserEntity = "Creating user entity for: {UserName}";
    public const string SuccessfullyCreatedUser = "Successfully created user: {UserName}, ID: {UserId}";
}
