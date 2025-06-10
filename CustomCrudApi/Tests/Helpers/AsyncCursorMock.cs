using MongoDB.Driver;

namespace CustomCrudApi.Tests.Helpers
{
    public class AsyncCursorMock<T> : IAsyncCursor<T>
    {
        private readonly IEnumerable<T> _items;
        private bool _moved;

        public AsyncCursorMock(IEnumerable<T> items)
        {
            _items = items;
        }

        public IEnumerable<T> Current => _items;

        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            if (!_moved)
            {
                _moved = true;
                return true;
            }
            return false;
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MoveNext(cancellationToken));
        }

        public void Dispose() { }
    }
}
