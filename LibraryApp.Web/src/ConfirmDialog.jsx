import { useEffect, useRef } from 'react'

function ConfirmDialog({ title, message, confirmLabel = 'Confirm', danger = false, onConfirm, onCancel }) {
  const confirmRef = useRef(null)

  
  useEffect(() => {
    confirmRef.current?.focus()

    function handleKey(event) {
      if (event.key === 'Escape') onCancel()
    }

    document.addEventListener('keydown', handleKey)
    return () => document.removeEventListener('keydown', handleKey)
  }, [onCancel])

  return (
    <div className="dialog-backdrop" onClick={onCancel}>
      <div
        className="dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="dialog-title"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 id="dialog-title">{title}</h2>
        <p className="muted">{message}</p>
        <div className="dialog-actions">
          <button type="button" className="btn-outline" onClick={onCancel}>
            Cancel
          </button>
          <button
            type="button"
            ref={confirmRef}
            className={`btn btn-inline ${danger ? 'btn-danger' : ''}`}
            onClick={onConfirm}
          >
            {confirmLabel}
          </button>
        </div>
      </div>
    </div>
  )
}

export default ConfirmDialog