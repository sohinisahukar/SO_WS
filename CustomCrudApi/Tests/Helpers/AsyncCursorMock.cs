using MongoDB.Driver;

namespace CustomCrudApi.Tests.Helpers
{
    /// <summary>
    /// Mock implementation of IAsyncCursor for testing MongoDB operations.
    /// </summary>
    /// <typeparam name="T">The type of items in the cursor.</typeparam>
    public class AsyncCursorMock<T> : IAsyncCursor<T>
    {
        private readonly IEnumerable<T> _items;
        private bool _moved;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCursorMock{T}"/> class.
        /// </summary>
        /// <param name="items">The items to be returned by the cursor.</param>
        /// <exception cref="ArgumentNullException">Thrown when items is null.</exception>
        public AsyncCursorMock(IEnumerable<T> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Gets the current batch of items.
        /// </summary>
        public IEnumerable<T> Current => _items;

        /// <summary>
        /// Moves to the next batch of items.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if there are more items; otherwise, false.</returns>
        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            if (!_moved)
            {
                _moved = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Asynchronously moves to the next batch of items.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task containing true if there are more items; otherwise, false.</returns>
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MoveNext(cancellationToken));
        }

        public void Dispose() { }
    }
}
