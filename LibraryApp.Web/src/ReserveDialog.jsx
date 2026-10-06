import { useEffect, useRef, useState } from 'react'

// Popup form for reserving a book.
// Customers send a request (picked up later); staff reserve directly for someone at the desk.
function ReserveDialog({ book, staff, onSubmit, onCancel }) {
  const [form, setForm] = useState({ borrowerName: '', borrowerPhone: '', customerUsername: '' })
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)
  const firstFieldRef = useRef(null)

  // Focus the first field and let Esc close the dialog.
  useEffect(() => {
    firstFieldRef.current?.focus()

    function handleKey(event) {
      if (event.key === 'Escape') onCancel()
    }

    document.addEventListener('keydown', handleKey)
    return () => document.removeEventListener('keydown', handleKey)
  }, [onCancel])

  function update(field) {
    return (e) => setForm({ ...form, [field]: e.target.value })
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setSaving(true)
    try {
      await onSubmit(form)
    } catch (err) {
      setError(err.message)
      setSaving(false)
    }
  }

  return (
    <div className="dialog-backdrop" onClick={onCancel}>
      <form
        className="dialog dialog-form"
        role="dialog"
        aria-modal="true"
        aria-labelledby="reserve-title"
        onClick={(e) => e.stopPropagation()}
        onSubmit={handleSubmit}
      >
        <h2 id="reserve-title">{staff ? 'Reserve for a customer' : 'Request this book'}</h2>
        <p className="muted">
          <strong>{book.title}</strong> · {book.author}
        </p>

        {error && <div className="alert">{error}</div>}

        {staff ? (
          <>
            <label>
              Customer name
              <input ref={firstFieldRef} value={form.borrowerName} onChange={update('borrowerName')} maxLength={100} required />
            </label>
            <label>
              Phone
              <input type="tel" value={form.borrowerPhone} onChange={update('borrowerPhone')} maxLength={20} required />
            </label>
            <label>
              Customer username <span className="muted small">(optional)</span>
              <input value={form.customerUsername} onChange={update('customerUsername')} maxLength={50} />
            </label>
            <p className="hint">The loan starts now. Add a username to show the booking in that customer&apos;s account.</p>
          </>
        ) : (
          <>
            <label>
              Phone <span className="muted small">(optional)</span>
              <input ref={firstFieldRef} type="tel" value={form.borrowerPhone} onChange={update('borrowerPhone')} maxLength={20} />
            </label>
            <p className="hint">The book is held for you. Your loan period starts when a librarian hands it over.</p>
          </>
        )}

        <div className="dialog-actions">
          <button type="button" className="btn-outline" onClick={onCancel}>
            Cancel
          </button>
          <button type="submit" className="btn btn-inline" disabled={saving}>
            {staff ? 'Reserve' : 'Send request'}
          </button>
        </div>
      </form>
    </div>
  )
}

export default ReserveDialog
