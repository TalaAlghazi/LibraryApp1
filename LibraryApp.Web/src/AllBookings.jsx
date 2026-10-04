import { useEffect, useState } from 'react'
import { api } from './api'
import { formatDate, statusInfo } from './bookingHelpers'
import ConfirmDialog from './ConfirmDialog'

function sortBookings(list) {
  return [...list].sort((a, b) => {
    const ra = a.status === 'ReturnRequested' ? 0 : 1
    const rb = b.status === 'ReturnRequested' ? 0 : 1
    return ra - rb || new Date(a.dueDate) - new Date(b.dueDate)
  })
}

function loadActive() {
  return api('/reservations?pageSize=200').then(sortBookings)
}

function AllBookings() {
  const [bookings, setBookings] = useState([])
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState(null)
  const [pending, setPending] = useState(null)

  useEffect(() => {
    loadActive()
      .then(setBookings)
      .catch((err) => setMessage({ type: 'error', text: err.message }))
      .finally(() => setLoading(false))
  }, [])

  async function confirmReturn() {
    const booking = pending
    setPending(null)
    setMessage(null)

    try {
      const result = await api(`/reservations/${booking.reservationId}/confirm-return`, { method: 'POST' })
      setMessage({ type: 'success', text: result.message })
      setBookings(await loadActive())
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    }
  }

  return (
    <>
      <h1 className="page-title">All bookings</h1>
      <p className="muted">Books currently out. Return requests are shown first.</p>

      {message && (
        <div className={`alert ${message.type === 'success' ? 'alert-success' : ''}`}>{message.text}</div>
      )}

      {loading ? (
        <p className="muted">Loading…</p>
      ) : bookings.length === 0 ? (
        <p className="empty">No books are out right now.</p>
      ) : (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Book</th>
                <th>Borrower</th>
                <th>Due</th>
                <th>Status</th>
                <th>
                  <span className="visually-hidden">Actions</span>
                </th>
              </tr>
            </thead>
            <tbody>
              {bookings.map((b) => {
                const info = statusInfo(b)
                const requested = b.status === 'ReturnRequested'
                return (
                  <tr key={b.reservationId} className={requested ? 'row-highlight' : ''}>
                    <td>
                      <strong>{b.bookTitle}</strong>
                      <div className="muted small">{b.bookAuthor}</div>
                    </td>
                    <td>
                      {b.borrowerName}
                      {b.borrowerPhone && <div className="muted small">{b.borrowerPhone}</div>}
                    </td>
                    <td>{formatDate(b.dueDate)}</td>
                    <td>
                      <span className={`status ${info.className}`}>{info.label}</span>
                    </td>
                    <td className="actions">
                      <button
                        type="button"
                        className={requested ? 'btn btn-inline btn-small' : 'btn-outline btn-small'}
                        onClick={() => setPending(b)}
                      >
                        {requested ? 'Confirm return' : 'Mark returned'}
                      </button>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}

      {pending && (
        <ConfirmDialog
          title="Confirm return"
          message={`“${pending.bookTitle}” will be marked as returned and the booking moves to history.`}
          confirmLabel="Confirm return"
          onConfirm={confirmReturn}
          onCancel={() => setPending(null)}
        />
      )}
    </>
  )
}

export default AllBookings