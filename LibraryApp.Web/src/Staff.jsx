import { useEffect, useState } from 'react'
import { api } from './api'

const emptyForm = { username: '', email: '', password: '', confirmPassword: '' }

// Admin page: list librarian accounts and create new ones.
function Staff() {
  const [librarians, setLibrarians] = useState([])
  const [loading, setLoading] = useState(true)
  const [form, setForm] = useState(emptyForm)
  const [message, setMessage] = useState(null)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    api('/users/librarians')
      .then(setLibrarians)
      .catch((err) => setMessage({ type: 'error', text: err.message }))
      .finally(() => setLoading(false))
  }, [])

  function update(field) {
    return (e) => setForm({ ...form, [field]: e.target.value })
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setMessage(null)

    if (form.password !== form.confirmPassword) {
      setMessage({ type: 'error', text: 'Passwords do not match.' })
      return
    }

    setSaving(true)
    try {
      const created = await api('/users/librarians', { method: 'POST', body: form })
      setLibrarians([...librarians, created].sort((a, b) => a.username.localeCompare(b.username)))
      setForm(emptyForm)
      setMessage({ type: 'success', text: `${created.username} can now sign in as a librarian.` })
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setSaving(false)
    }
  }

  return (
    <>
      <h1 className="page-title">Staff</h1>
      <p className="muted">Create accounts for librarians. Customers sign up themselves from the registration page.</p>

      {message && (
        <div className={`alert ${message.type === 'success' ? 'alert-success' : ''}`}>{message.text}</div>
      )}

      <div className="profile-grid">
        <form className="form-card" onSubmit={handleSubmit}>
          <h2>Add a librarian</h2>
          <label>
            Username
            <input value={form.username} onChange={update('username')} minLength={3} maxLength={50} required />
          </label>
          <label>
            Email
            <input type="email" value={form.email} onChange={update('email')} required />
          </label>
          <label>
            Password
            <input
              type="password"
              value={form.password}
              onChange={update('password')}
              autoComplete="new-password"
              minLength={8}
              required
            />
          </label>
          <label>
            Confirm password
            <input
              type="password"
              value={form.confirmPassword}
              onChange={update('confirmPassword')}
              autoComplete="new-password"
              required
            />
          </label>
          <button type="submit" className="btn btn-inline" disabled={saving}>
            Create librarian
          </button>
        </form>

        <div className="form-card">
          <h2>Librarians</h2>
          {loading ? (
            <p className="muted">Loading…</p>
          ) : librarians.length === 0 ? (
            <p className="muted">No librarian accounts yet.</p>
          ) : (
            <ul className="staff-list">
              {librarians.map((librarian) => (
                <li key={librarian.id}>
                  <strong>{librarian.username}</strong>
                  <span className="muted small">{librarian.email}</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </>
  )
}

export default Staff