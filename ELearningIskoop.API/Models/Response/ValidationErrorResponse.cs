using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.API.Models.Response
{
    public class ValidationErrorResponse : ErrorResponse
    {
        public ValidationErrorResponse(IDictionary<string, string[]> errors)
        {
            Title = "Validation Failed";
            Status = 400;
            Detail = "One or more validation errors occurred";
            ErrorCode = "VALIDATION_ERROR";
            Errors = errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
