namespace Games.Common.Exceptions
{
    public class GuardValidationResult
    {
        public GuardValidationResult(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public string Name { get; private set; }

        public string Message { get; private set; }
    }
}