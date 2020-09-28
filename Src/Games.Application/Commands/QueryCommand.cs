using Newtonsoft.Json;

namespace Games.Application.Commands
{
    public abstract class QueryCommand
    {
        public string Search { get; set; }

        public string Sort { get; set; }

        public string Order { get; set; }

        public int Page { get; set; }

        public int Size { get; set; }
    }
}