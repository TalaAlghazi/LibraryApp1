import { useEffect, useRef, useState } from 'react'
import { BookIcon, MenuIcon } from './Icons'

const accountPages = ['bookings', 'history', 'profile']

function Header({ user, ready, page, onNavigate, onLogout, onShowLogin, onShowRegister }) {
  const [menuOpen, setMenuOpen] = useState(false) // mobile menu
  const [userOpen, setUserOpen] = useState(false) // account dropdown
  const userMenuRef = useRef(null)

  // Close the account dropdown on an outside click or Esc.
  useEffect(() => {
    if (!userOpen) return

    function handleClick(event) {
      if (!userMenuRef.current?.contains(event.target)) setUserOpen(false)
    }
    function handleKey(event) {
      if (event.key === 'Escape') setUserOpen(false)
    }

    document.addEventListener('mousedown', handleClick)
    document.addEventListener('keydown', handleKey)
    return () => {
      document.removeEventListener('mousedown', handleClick)
      document.removeEventListener('keydown', handleKey)
    }
  }, [userOpen])

  function closeMenus() {
    setMenuOpen(false)
    setUserOpen(false)
  }

  function go(target) {
    closeMenus()
    onNavigate(target)
  }

  const links = [
    { id: 'books', label: 'Books', active: page === 'books' },
    { id: 'bookings', label: 'My Account', active: accountPages.includes(page) },
    ...(user?.role === 'Admin' ? [{ id: 'all', label: 'All Bookings', active: page === 'all' }] : []),
  ]

  return (
    <header className="site-header">
      <div className="container site-header-inner">
        <button type="button" className="site-brand" onClick={() => (user ? go('books') : onShowLogin())}>
          <BookIcon />
          <span>Library</span>
        </button>

        {ready && (
          <>
            <button
              type="button"
              className="menu-toggle"
              aria-label="Toggle navigation"
              aria-expanded={menuOpen}
              aria-controls="site-menu"
              onClick={() => setMenuOpen(!menuOpen)}
            >
              <MenuIcon />
            </button>

            <div id="site-menu" className={`site-menu ${menuOpen ? 'is-open' : ''}`}>
              {user ? (
                <>
                  <nav aria-label="Main">
                    <ul className="site-nav">
                      {links.map((link) => (
                        <li key={link.id}>
                          <button
                            type="button"
                            aria-current={link.active ? 'page' : undefined}
                            onClick={() => go(link.id)}
                          >
                            {link.label}
                          </button>
                        </li>
                      ))}
                    </ul>
                  </nav>

                  <div className="user-menu" ref={userMenuRef}>
                    <button
                      type="button"
                      className="user-button"
                      aria-haspopup="true"
                      aria-expanded={userOpen}
                      onClick={() => setUserOpen(!userOpen)}
                    >
                      <span className="avatar" aria-hidden="true">
                        {user.username.charAt(0).toUpperCase()}
                      </span>
                      <span>{user.username}</span>
                    </button>

                    {userOpen && (
                      <div className="dropdown">
                        <p className="dropdown-head">Signed in as {user.role}</p>
                        <button type="button" onClick={() => go('bookings')}>
                          My Bookings
                        </button>
                        <button type="button" onClick={() => go('profile')}>
                          My Profile
                        </button>
                        <hr />
                        <button
                          type="button"
                          className="logout"
                          onClick={() => {
                            closeMenus()
                            onLogout()
                          }}
                        >
                          Logout
                        </button>
                      </div>
                    )}
                  </div>
                </>
              ) : (
                <div className="guest-actions">
                  <button
                    type="button"
                    className="link-button plain"
                    onClick={() => {
                      closeMenus()
                      onShowLogin()
                    }}
                  >
                    Sign in
                  </button>
                  <button
                    type="button"
                    className="btn btn-inline btn-small"
                    onClick={() => {
                      closeMenus()
                      onShowRegister()
                    }}
                  >
                    Create account
                  </button>
                </div>
              )}
            </div>
          </>
        )}
      </div>
    </header>
  )
}

export default Header
