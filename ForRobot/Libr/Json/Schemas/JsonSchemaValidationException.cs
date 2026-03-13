using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Schema;

namespace ForRobot.Libr.Json.Schemas
{
    public class ValidationErrorInfo
    {
        public string Message { get; set; }
        public string Path { get; set; }
        public SchemaValidationEventArgs ErrorDetails { get; set; }

        public override string ToString() => $"Path: {Path}, Message: {Message}";
    }

    public class JsonSchemaValidationException : Exception
    {
        public IList<ValidationErrorInfo> ValidationErrors { get; }
        public string JsonData { get; }
        public string SchemaTitle { get; }

        public JsonSchemaValidationException(string schemaTitle, ValidationErrorInfo error, string jsonData, Exception innerException = null)
           : this(schemaTitle, new List<ValidationErrorInfo> { error }, jsonData, innerException) { }

        public JsonSchemaValidationException(string schemaTitle, IList<ValidationErrorInfo> errors, string jsonData, Exception innerException = null)
            : base(BuildMessage(schemaTitle, errors), innerException)
        {
            if (string.IsNullOrEmpty(schemaTitle))
                throw new ArgumentNullException(nameof(schemaTitle));

            ValidationErrors = errors ?? throw new ArgumentNullException(nameof(errors));
            JsonData = jsonData;
            SchemaTitle = schemaTitle;
        }

        private static string BuildMessage(string schemaTitle, IEnumerable<ValidationErrorInfo> errors)
        {
            var messages = errors?.Select(e => e.Message) ?? Enumerable.Empty<string>();
            return $"JSON validation failed for schema '{schemaTitle}'. Errors: {string.Join("; ", messages)}";
        }
    }
}
