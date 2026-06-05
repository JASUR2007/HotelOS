import { type ReactNode, useEffect } from 'react';

interface ModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  size?: 'sm' | 'md' | 'lg';
}

export default function Modal({ open, onClose, title, children, size = 'md' }: ModalProps) {
  useEffect(() => {
    if (open) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
    }
    return () => { document.body.style.overflow = ''; };
  }, [open]);

  if (!open) return null;

  const widths = { sm: 'max-w-md', md: 'max-w-lg', lg: 'max-w-2xl' };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm">
      <div className={`w-full ${widths[size]} mx-4 rounded-xl border border-primary/10 bg-white shadow-2xl`}>
        <div className="flex items-center justify-between border-b border-primary/10 px-6 py-5">
          <h2 className="text-lg font-semibold text-primary">{title}</h2>
          <button onClick={onClose} className="rounded-lg p-1 text-primary/40 hover:bg-primary/5 hover:text-primary">
            ✕
          </button>
        </div>
        <div className="px-6 py-5">{children}</div>
      </div>
    </div>
  );
}

export function ConfirmDeleteModal({
  open,
  onClose,
  onConfirm,
  title,
  message,
}: {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
}) {
  return (
    <Modal open={open} onClose={onClose} title={title} size="sm">
      <p className="text-sm text-primary/70">{message}</p>
      <div className="mt-6 flex justify-end gap-3">
        <button onClick={onClose} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">
          Cancel
        </button>
        <button onClick={onConfirm} className="rounded-lg bg-rose-600 px-5 py-2.5 text-sm text-white hover:bg-rose-700">
          Delete
        </button>
      </div>
    </Modal>
  );
}
