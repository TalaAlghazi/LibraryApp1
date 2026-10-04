import { useState } from 'react'
import { api } from './api'
import AuthArt from './AuthArt'
import AuthField from './AuthField'
import { LockIcon, MailIcon, UserIcon } from './Icons'

function Register({ onRegister, onShowLogin }) {
  const [form, setForm] = useState({ username: '', email: '', password: '', confirmPassword: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  function update(field) {
    return (e) => setForm({ ...form, [field]: e.target.value })
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (form.password !== form.confirmPassword) {
      setError('Passwords do not match.')
      return
    }

    setLoading(true)
    try {
      const user = await api('/auth/register', { method: 'POST', body: form })
      onRegister(user)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-shell">
      <AuthArt title="Your next story begins here." />

      <div className="auth-form-side">
        <form className="auth-form" onSubmit={handleSubmit}>
          <h1 className="auth-title">Create your account</h1>
          <p className="auth-subtitle">Join the library to reserve and track your books</p>

          {error && (
            <div className="alert" role="alert">
              {error}
            </div>
          )}

          <AuthField
            id="register-username"
            label="Username"
            icon={UserIcon}
            value={form.username}
            onChange={update('username')}
            autoComplete="username"
            minLength={3}
            maxLength={50}
            autoFocus
            required
          />
          <AuthField
            id="register-email"
            label="Email"
            icon={MailIcon}
            type="email"
            value={form.email}
            onChange={update('email')}
            autoComplete="email"
            required
          />
          <AuthField
            id="register-password"
            label="Password"
            icon={LockIcon}
            type="password"
            value={form.password}
            onChange={update('password')}
            autoComplete="new-password"
            minLength={8}
            required
          />
          <p className="auth-hint">At least 8 characters, including a letter and a number.</p>
          <AuthField
            id="register-confirm"
            label="Confirm password"
            icon={LockIcon}
            type="password"
            value={form.confirmPassword}
            onChange={update('confirmPassword')}
            autoComplete="new-password"
            required
          />

          <button type="submit" className="btn auth-submit" disabled={loading}>
            {loading ? 'Creating account…' : 'Create Account'}
          </button>

          <p className="auth-alt">
            Already have an account?{' '}
            <button type="button" className="link-button" onClick={onShowLogin}>
              Sign in
            </button>
          </p>
        </form>
      </div>
    </div>
  )
}

export default Register
