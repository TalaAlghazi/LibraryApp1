// Shared helpers for showing bookings.
export function formatDate(value) {
  return value ? new Date(value).toLocaleDateString() : '—'
}

export function isStaffRole(role) {
  return role === 'Admin' || role === 'Librarian'
}

export function statusInfo(booking) {
  if (booking.status === 'Pending') return { label: 'Awaiting pickup', className: 'is-requested' }
  if (booking.status === 'Rejected') return { label: 'Rejected', className: 'is-overdue' }
  if (booking.status === 'Returned') return { label: 'Returned', className: 'is-returned' }
  if (booking.status === 'ReturnRequested') return { label: 'Return requested', className: 'is-requested' }
  if (new Date(booking.dueDate) < new Date()) return { label: 'Overdue', className: 'is-overdue' }
  return { label: 'Active', className: 'is-available' }
}
