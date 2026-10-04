import { useState } from 'react'
import { api } from './api'
import AuthArt from './AuthArt'
import AuthField from './AuthField'
import { LockIcon, UserIcon } from './Icons'

function Login({ onLogin, onShowRegister }) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setLoading(true)

    try {
      const user = await api('/auth/login', {
        method: 'POST',
        body: { username, password },
      })
      onLogin(user)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-shell">
      <AuthArt title="A world of stories awaits." />

      <div className="auth-form-side">
        <form className="auth-form" onSubmit={handleSubmit}>
          <h1 className="auth-title">Welcome back</h1>
          <p className="auth-subtitle">Sign in to your library account</p>

          {error && (
            <div className="alert" role="alert">
              {error}
            </div>
          )}

          <AuthField
            id="login-username"
            label="Username"
            icon={UserIcon}
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoComplete="username"
            autoFocus
            required
          />
          <AuthField
            id="login-password"
            label="Password"
            icon={LockIcon}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
            required
          />

          <button type="submit" className="btn auth-submit" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign In'}
          </button>

          <p className="auth-alt">
            New here?{' '}
            <button type="button" className="link-button" onClick={onShowRegister}>
              Create an account
            </button>
          </p>
        </form>
      </div>
    </div>
  )
}

export default Login
