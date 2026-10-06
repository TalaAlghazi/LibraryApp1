import { useEffect, useState } from 'react'
import { api } from './api'
import { formatDate } from './bookingHelpers'
import ConfirmDialog from './ConfirmDialog'

function loadRequests() {
  return api('/reservations/pending-requests').then((list) =>
    [...list].sort((a, b) => new Date(a.reservedAt) - new Date(b.reservedAt)),
  )
}

// Staff page: customer requests waiting to be handed over.
function Requests() {
  const [requests, setRequests] = useState([])
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState(null)
  const [pending, setPending] = useState(null) // { request, action }

  useEffect(() => {
    loadRequests()
      .then(setRequests)
      .catch((err) => setMessage({ type: 'error', text: err.message }))
      .finally(() => setLoading(false))
  }, [])

  async function confirmAction() {
    const { request, action } = pending
    setPending(null)
    setMessage(null)

    try {
      const result = await api(`/reservations/${request.reservationId}/${action}`, { method: 'POST' })
      setMessage({ type: 'success', text: result.message })
      setRequests(await loadRequests())
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    }
  }

  return (
    <>
      <h1 className="page-title">Pending requests</h1>
      <p className="muted">Books customers asked for. Hand the book over to start the loan, or reject the request.</p>

      {message && (
        <div className={`alert ${message.type === 'success' ? 'alert-success' : ''}`}>{message.text}</div>
      )}

      {loading ? (
        <p className="muted">Loading…</p>
      ) : requests.length === 0 ? (
        <p className="empty">No pending requests.</p>
      ) : (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Book</th>
                <th>Customer</th>
                <th>Requested</th>
                <th>
                  <span className="visually-hidden">Actions</span>
                </th>
              </tr>
            </thead>
            <tbody>
              {requests.map((r) => (
                <tr key={r.reservationId}>
                  <td>
                    <strong>{r.bookTitle}</strong>
                    <div className="muted small">{r.bookAuthor}</div>
                  </td>
                  <td>
                    {r.borrowerName}
                    {r.borrowerPhone && <div className="muted small">{r.borrowerPhone}</div>}
                  </td>
                  <td>{formatDate(r.reservedAt)}</td>
                  <td className="actions">
                    <button
                      type="button"
                      className="btn btn-inline btn-small"
                      onClick={() => setPending({ request: r, action: 'hand-over' })}
                    >
                      Hand over
                    </button>{' '}
                    <button
                      type="button"
                      className="btn-outline btn-small"
                      onClick={() => setPending({ request: r, action: 'reject' })}
                    >
                      Reject
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {pending && (
        <ConfirmDialog
          title={pending.action === 'hand-over' ? 'Hand over book' : 'Reject request?'}
          message={
            pending.action === 'hand-over'
              ? `Give “${pending.request.bookTitle}” to ${pending.request.borrowerName}. The loan period starts now.`
              : `“${pending.request.bookTitle}” will become available to other customers again.`
          }
          confirmLabel={pending.action === 'hand-over' ? 'Hand over' : 'Reject'}
          danger={pending.action === 'reject'}
          onConfirm={confirmAction}
          onCancel={() => setPending(null)}
        />
      )}
    </>
  )
}

export default Requests
