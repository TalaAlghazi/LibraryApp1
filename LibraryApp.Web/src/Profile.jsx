import { useEffect, useState } from 'react'
import { api } from './api'
import { formatDate } from './bookingHelpers'

const emptyPasswords = { currentPassword: '', newPassword: '', confirmPassword: '' }

function Profile({ onUserChange }) {
  const [profile, setProfile] = useState(null)
  const [form, setForm] = useState({ username: '', email: '' })
  const [passwords, setPasswords] = useState(emptyPasswords)
  const [message, setMessage] = useState(null)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    api('/account/profile')
      .then((data) => {
        setProfile(data)
        setForm({ username: data.username, email: data.email })
      })
      .catch((err) => setMessage({ type: 'error', text: err.message }))
  }, [])

  async function saveProfile(event) {
    event.preventDefault()
    setMessage(null)
    setSaving(true)

    try {
      const updated = await api('/account/profile', { method: 'PUT', body: form })
      setProfile({ ...profile, username: updated.username, email: updated.email })
      onUserChange(updated) // keeps the name in the top bar in sync
      setMessage({ type: 'success', text: 'Profile updated successfully.' })
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setSaving(false)
    }
  }

  async function changePassword(event) {
    event.preventDefault()
    setMessage(null)

    if (passwords.newPassword !== passwords.confirmPassword) {
      setMessage({ type: 'error', text: 'Passwords do not match.' })
      return
    }

    setSaving(true)
    try {
      const result = await api('/account/change-password', { method: 'POST', body: passwords })
      setPasswords(emptyPasswords)
      setMessage({ type: 'success', text: result.message })
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setSaving(false)
    }
  }

  if (!profile) {
    return message ? <div className="alert">{message.text}</div> : <p className="muted">Loading…</p>
  }

  return (
    <>
      <h1 className="page-title">My profile</h1>
      <p className="muted">
        {profile.role} · Member since {formatDate(profile.memberSince)}
      </p>

      {message && (
        <div className={`alert ${message.type === 'success' ? 'alert-success' : ''}`}>{message.text}</div>
      )}

      <div className="profile-grid">
        <form className="form-card" onSubmit={saveProfile}>
          <h2>Account details</h2>
          <label>
            Username
            <input
              value={form.username}
              onChange={(e) => setForm({ ...form, username: e.target.value })}
              minLength={3}
              maxLength={50}
              required
            />
          </label>
          <label>
            Email
            <input
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              required
            />
          </label>
          <button type="submit" className="btn btn-inline" disabled={saving}>
            Save changes
          </button>
        </form>

        <form className="form-card" onSubmit={changePassword}>
          <h2>Change password</h2>
          <label>
            Current password
            <input
              type="password"
              value={passwords.currentPassword}
              onChange={(e) => setPasswords({ ...passwords, currentPassword: e.target.value })}
              autoComplete="current-password"
              required
            />
          </label>
          <label>
            New password
            <input
              type="password"
              value={passwords.newPassword}
              onChange={(e) => setPasswords({ ...passwords, newPassword: e.target.value })}
              autoComplete="new-password"
              minLength={8}
              required
            />
          </label>
          <label>
            Confirm new password
            <input
              type="password"
              value={passwords.confirmPassword}
              onChange={(e) => setPasswords({ ...passwords, confirmPassword: e.target.value })}
              autoComplete="new-password"
              required
            />
          </label>
          <button type="submit" className="btn btn-inline" disabled={saving}>
            Change password
          </button>
        </form>
      </div>
    </>
  )
}

export default Profile