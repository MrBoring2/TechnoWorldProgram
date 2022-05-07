namespace TechnoWorld_API.Models
{
    public class ValidationResult
    {
        public ValidationResult(bool result, string message)
        {
            Result = result;
            Message = message;
        }

        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
