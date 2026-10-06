import { useEffect, useState } from 'react'
import { api } from './api'
import { formatDate, statusInfo } from './bookingHelpers'
import ConfirmDialog from './ConfirmDialog'

function MyBookings({ mode }) {
  const isHistory = mode === 'history'

  const [bookings, setBookings] = useState([])
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState(null)
  const [pending, setPending] = useState(null) // booking waiting for confirmation

  useEffect(() => {
    api(isHistory ? '/account/history' : '/account/bookings')
      .then(setBookings)
      .catch((err) => setMessage({ type: 'error', text: err.message }))
      .finally(() => setLoading(false))
  }, [isHistory])

  async function confirmReturn() {
    const booking = pending
    setPending(null)
    setMessage(null)

    try {
      const result = await api(`/account/bookings/${booking.reservationId}/return-request`, { method: 'POST' })
      setMessage({ type: 'success', text: result.message })
      setBookings(await api('/account/bookings'))
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    }
  }

  return (
    <>
      <h1 className="page-title">{isHistory ? 'Booking history' : 'My bookings'}</h1>
      <p className="muted">
        {isHistory
          ? 'Returned books and rejected requests.'
          : 'Your requests and the books you have. Request a return when you hand a book back.'}
      </p>

      {message && (
        <div className={`alert ${message.type === 'success' ? 'alert-success' : ''}`}>{message.text}</div>
      )}

      {loading ? (
        <p className="muted">Loading…</p>
      ) : bookings.length === 0 ? (
        <p className="empty">{isHistory ? 'Nothing here yet.' : 'You have no current bookings.'}</p>
      ) : (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Book</th>
                <th>Reserved</th>
                <th>{isHistory ? 'Closed' : 'Due'}</th>
                {isHistory && <th>Fine</th>}
                <th>Status</th>
                {!isHistory && (
                  <th>
                    <span className="visually-hidden">Actions</span>
                  </th>
                )}
              </tr>
            </thead>
            <tbody>
              {bookings.map((b) => {
                const info = statusInfo(b)
                return (
                  <tr key={b.reservationId}>
                    <td>
                      <strong>{b.bookTitle}</strong>
                      <div className="muted small">{b.bookAuthor}</div>
                    </td>
                    <td>{formatDate(b.reservedAt)}</td>
                    <td>
                      {isHistory ? formatDate(b.returnedAt) : b.status === 'Pending' ? 'After pickup' : formatDate(b.dueDate)}
                    </td>
                    {isHistory && <td>{b.fine > 0 ? `$${b.fine.toFixed(2)}` : '—'}</td>}
                    <td>
                      <span className={`status ${info.className}`}>{info.label}</span>
                    </td>
                    {!isHistory && (
                      <td className="actions">
                        {b.status === 'Active' ? (
                          <button type="button" className="btn-outline btn-small" onClick={() => setPending(b)}>
                            Request return
                          </button>
                        ) : (
                          <span className="muted small">
                            {b.status === 'Pending' ? 'Collect at the library' : 'Waiting for staff'}
                          </span>
                        )}
                      </td>
                    )}
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}

      {pending && (
        <ConfirmDialog
          title="Return this book?"
          message={`An admin will confirm the return of “${pending.bookTitle}” once the book is received.`}
          confirmLabel="Request return"
          onConfirm={confirmReturn}
          onCancel={() => setPending(null)}
        />
      )}
    </>
  )
}

export default MyBookings
