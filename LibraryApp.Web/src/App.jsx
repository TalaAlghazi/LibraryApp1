import { useEffect, useState } from 'react'
import AllBookings from './AllBookings'
import { api } from './api'
import { isStaffRole } from './bookingHelpers'
import Books from './Books'
import Header from './Header'
import Login from './Login'
import MyBookings from './MyBookings'
import Profile from './Profile'
import Register from './Register'
import Requests from './Requests'
import Staff from './Staff'

const accountTabs = [
  { id: 'bookings', label: 'Current Bookings' },
  { id: 'history', label: 'Booking History' },
  { id: 'profile', label: 'My Profile' },
]

// Pill tabs shown on the "My Account" pages.
function AccountNav({ page, onNavigate }) {
  return (
    <nav className="account-nav" aria-label="My account">
      {accountTabs.map((tab) => (
        <button
          key={tab.id}
          type="button"
          aria-current={page === tab.id ? 'page' : undefined}
          onClick={() => onNavigate(tab.id)}
        >
          {tab.label}
        </button>
      ))}
    </nav>
  )
}

function App() {
  const [user, setUser] = useState(null)
  const [checking, setChecking] = useState(true)
  const [page, setPage] = useState('books')
  const [showRegister, setShowRegister] = useState(false)

  // On first load, ask the API whether the login cookie is still valid.
  useEffect(() => {
    api('/auth/me')
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setChecking(false))
  }, [])

  function handleSignedIn(signedInUser) {
    setUser(signedInUser)
    setShowRegister(false)
    setPage('books')
  }

  async function handleLogout() {
    await api('/auth/logout', { method: 'POST' })
    setUser(null)
    setShowRegister(false)
    setPage('books')
  }

  let content
  if (checking) {
    content = <p className="center-note">Loading…</p>
  } else if (!user) {
    content = showRegister ? (
      <Register onRegister={handleSignedIn} onShowLogin={() => setShowRegister(false)} />
    ) : (
      <Login onLogin={handleSignedIn} onShowRegister={() => setShowRegister(true)} />
    )
  } else {
    const isAccountPage = accountTabs.some((tab) => tab.id === page)
    content = (
      <>
        {isAccountPage && <AccountNav page={page} onNavigate={setPage} />}
        {page === 'books' && <Books user={user} />}
        {(page === 'bookings' || page === 'history') && (
          <MyBookings key={page} mode={page === 'history' ? 'history' : 'current'} />
        )}
        {page === 'profile' && <Profile onUserChange={setUser} />}
        {page === 'requests' && isStaffRole(user.role) && <Requests />}
        {page === 'all' && isStaffRole(user.role) && <AllBookings />}
        {page === 'staff' && user.role === 'Admin' && <Staff />}
      </>
    )
  }

  return (
    <>
      <Header
        user={user}
        ready={!checking}
        page={page}
        onNavigate={setPage}
        onLogout={handleLogout}
        onShowLogin={() => setShowRegister(false)}
        onShowRegister={() => setShowRegister(true)}
      />

      <main className="site-main">
        <div className="container">{content}</div>
      </main>

      <footer className="site-footer">
        <div className="container">© {new Date().getFullYear()} · Library</div>
      </footer>
    </>
  )
}

export default App
