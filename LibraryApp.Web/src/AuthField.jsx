import { useState } from 'react'
import { EyeIcon } from './Icons'

// Input with a leading icon; password fields also get a show/hide button.
function AuthField({ id, label, icon: Icon, type = 'text', ...inputProps }) {
  const isPassword = type === 'password'
  const [visible, setVisible] = useState(false)

  return (
    <div className="auth-field">
      <label htmlFor={id} className="visually-hidden">
        {label}
      </label>
      <Icon className="auth-field-icon" />
      <input
        id={id}
        type={isPassword && visible ? 'text' : type}
        placeholder={label}
        className={isPassword ? 'has-toggle' : undefined}
        {...inputProps}
      />
      {isPassword && (
        <button
          type="button"
          className="auth-toggle"
          aria-label={visible ? 'Hide password' : 'Show password'}
          aria-pressed={visible}
          onClick={() => setVisible(!visible)}
        >
          <EyeIcon crossed={visible} />
        </button>
      )}
    </div>
  )
}

export default AuthField
