export function formatDate(value) {
  return value ? new Date(value).toLocaleDateString() : '—'
}

export function statusInfo(booking) {
  if (booking.status === 'Returned') return { label: 'Returned', className: 'is-returned' }
  if (booking.status === 'ReturnRequested') return { label: 'Return requested', className: 'is-requested' }
  if (new Date(booking.dueDate) < new Date()) return { label: 'Overdue', className: 'is-overdue' }
  return { label: 'Active', className: 'is-available' }
}