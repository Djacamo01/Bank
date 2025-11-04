namespace Lafise.API.utils
{
    public class ErrorResponseDto
    {
        public bool Error { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
    }
}

