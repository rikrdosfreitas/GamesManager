using System.Collections.Generic;

namespace Games.Application.Models
{
    public class ResponseViewModel<T> where T : IListViewModel
    {
        public ResponseViewModel(int records, IEnumerable<T> data)
        {
            Records = records;
            Data = data;
        }

        public int Records { get; }

        public IEnumerable<T> Data { get; }
    }
}