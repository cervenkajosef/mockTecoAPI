namespace mockTecoAPI.Models.Error
{
    public class Errors
    {
        private ErrorResponse ErrorTemplate()
        {
            var jsonData = new ErrorResponse
            {
                error = new ErrorDetails
                {
                    code = "",
                    message = "",
                    time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };
            return jsonData;
        }

        public ErrorResponse ErrorNotFound(string param)
        {
            var jsonError = ErrorTemplate();
            jsonError.error.code = "400.005";
            jsonError.error.message = $"Variable {param} is not found";
            return jsonError;
        }
        public ErrorResponse BadParameters()
        {
            var jsonError = ErrorTemplate();
            jsonError.error.code = "400.001";
            jsonError.error.message = "Bad parameters (any PLC variable is not specified)";
            return jsonError;
        }
    }
}
