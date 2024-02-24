namespace Tokengram.Models.Validation
{
    public class ValidationError
    {
        public string Property { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
