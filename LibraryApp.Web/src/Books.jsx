import { useEffect, useState } from 'react'
import { api } from './api'
import ConfirmDialog from './ConfirmDialog'
import HeroArt from './HeroArt'
import { SearchIcon, SprigIcon } from './Icons'

function fetchBooks(term) {
  const query = term ? `?search=${encodeURIComponent(term)}` : ''
  return api(`/books${query}`)
}

function Books({ user }) {
  const isAdmin = user.role === 'Admin'

  const [books, setBooks] = useState([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [activeSearch, setActiveSearch] = useState('')
  const [message, setMessage] = useState(null) // { type: 'success' | 'error', text }
  const [phones, setPhones] = useState({}) // phone typed on each book card
  const [newBook, setNewBook] = useState({ title: '', author: '' })
  const [pending, setPending] = useState(null) // action waiting for confirmation

  // First load: available books.
  useEffect(() => {
    fetchBooks('')
      .then(setBooks)
      .catch((err) => setMessage({ type: 'error', text: err.message }))
      .finally(() => setLoading(false))
  }, [])

  async function reload(term = activeSearch) {
    setLoading(true)
    try {
      setBooks(await fetchBooks(term))
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setLoading(false)
    }
  }

  // Runs an API call, shows the result and refreshes the list. Returns true on success.
  async function runAction(action, successText) {
    setMessage(null)
    try {
      const result = await action()
      setMessage({ type: 'success', text: result?.message ?? successText })
      await reload()
      return true
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
      return false
    }
  }

  function handleSearch(event) {
    event.preventDefault()
    const term = search.trim()
    setActiveSearch(term)
    reload(term)
  }

  function clearSearch() {
    setSearch('')
    setActiveSearch('')
    reload('')
  }

  function askReserve(book) {
    setPending({
      title: 'Reserve book',
      message: `Do you want to reserve “${book.title}”?`,
      confirmLabel: 'Reserve',
      successText: 'Book reserved.',
      run: () =>
        api(`/books/${book.id}/reserve`, {
          method: 'POST',
          body: { borrowerPhone: phones[book.id] ?? '' },
        }),
    })
  }

  function askDelete(book) {
    setPending({
      title: 'Delete book?',
      message: `“${book.title}” will be permanently removed from the library.`,
      confirmLabel: 'Delete',
      danger: true,
      successText: 'Book deleted.',
      run: () => api(`/books/${book.id}`, { method: 'DELETE' }),
    })
  }

  async function confirmPending() {
    const action = pending
    setPending(null)
    await runAction(action.run, action.successText)
  }

  async function handleAdd(event) {
    event.preventDefault()
    const ok = await runAction(() => api('/books', { method: 'POST', body: newBook }), 'Book added.')
    if (ok) setNewBook({ title: '', author: '' })
  }

  return (
    <section className="lib-panel">
      <div className="hero">
        <div>
          <h1 className="hero-title">Your next chapter starts here.</h1>
          <p className="hero-subtitle">Discover, reserve, and enjoy your next read.</p>

          <form className="hero-search" onSubmit={handleSearch} role="search">
            <span className="hero-search-icon">
              <SearchIcon width="18" height="18" />
            </span>
            <label htmlFor="book-search" className="visually-hidden">
              Search books by title
            </label>
            <input
              id="book-search"
              type="search"
              placeholder="Search by title..."
              autoComplete="off"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            <button type="submit" className="btn">
              Search
            </button>
          </form>

          {activeSearch && (
            <p className="search-summary">
              Showing results for “{activeSearch}” ·{' '}
              <button type="button" className="link-button" onClick={clearSearch}>
                Clear search
              </button>
            </p>
          )}
        </div>

        <HeroArt />
      </div>

      {message && (
        <div className={`alert ${message.type === 'success' ? 'alert-success' : ''}`} role="status">
          {message.text}
        </div>
      )}

      {isAdmin && (
        <div className="admin-bar">
          <h2 className="admin-bar-title">Add a book</h2>
          <form className="admin-bar-form" onSubmit={handleAdd}>
            <input
              placeholder="Title"
              aria-label="Book title"
              maxLength={200}
              value={newBook.title}
              onChange={(e) => setNewBook({ ...newBook, title: e.target.value })}
              required
            />
            <input
              placeholder="Author"
              aria-label="Book author"
              maxLength={200}
              value={newBook.author}
              onChange={(e) => setNewBook({ ...newBook, author: e.target.value })}
              required
            />
            <button type="submit" className="btn btn-inline">
              Add Book
            </button>
          </form>
        </div>
      )}

      {loading ? (
        <p className="muted">Loading books…</p>
      ) : books.length === 0 ? (
        <div className="empty-state">
          <p className="empty-state-title">No books found</p>
          <p>
            {activeSearch ? (
              <>
                Nothing matches “{activeSearch}”.{' '}
                <button type="button" className="link-button" onClick={clearSearch}>
                  See all books
                </button>
              </>
            ) : (
              'There are no available books right now. Please check back soon.'
            )}
          </p>
        </div>
      ) : (
        <div className="book-grid">
          {books.map((book) => (
            <article key={book.id} className="book-card">
              <div className="book-card-body">
                <div className={`book-cover cover-tone-${book.id % 5}`} aria-hidden="true">
                  <span className="book-cover-title">{book.title}</span>
                  <SprigIcon className="book-cover-mark" />
                  <span className="book-cover-author">{book.author}</span>
                </div>

                <div className="book-info">
                  <h2 className="book-title">{book.title}</h2>
                  <p className="book-author">{book.author}</p>
                  <span className={`book-status ${book.isAvailable ? 'is-available' : 'is-reserved'}`}>
                    {book.isAvailable ? 'Available' : 'Reserved'}
                  </span>
                </div>
              </div>

              <div className="book-card-actions">
                {book.isAvailable ? (
                  <div className="book-reserve">
                    <input
                      type="tel"
                      placeholder="Phone (optional)"
                      aria-label={`Phone number for reserving ${book.title}`}
                      maxLength={20}
                      value={phones[book.id] ?? ''}
                      onChange={(e) => setPhones({ ...phones, [book.id]: e.target.value })}
                    />
                    <button type="button" className="btn" onClick={() => askReserve(book)}>
                      Reserve
                    </button>
                  </div>
                ) : (
                  <p className="book-card-note">Currently reserved</p>
                )}

                {isAdmin && (
                  <button type="button" className="link-button danger" onClick={() => askDelete(book)}>
                    Delete book
                  </button>
                )}
              </div>
            </article>
          ))}
        </div>
      )}

      {pending && (
        <ConfirmDialog
          title={pending.title}
          message={pending.message}
          confirmLabel={pending.confirmLabel}
          danger={pending.danger}
          onConfirm={confirmPending}
          onCancel={() => setPending(null)}
        />
      )}
    </section>
  )
}

export default Books
