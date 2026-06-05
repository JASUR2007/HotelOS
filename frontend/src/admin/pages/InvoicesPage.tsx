import { useEffect, useState } from 'react';
import GenericListPage from './GenericListPage';
import StatusBadge from '../components/StatusBadge';

interface Invoice {
  id: number;
  invoiceNumber: string;
  guestName: string;
  totalAmount: number;
  status: string;
}

export default function InvoicesPage() {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
    fetch(`${apiBaseUrl}/payments/invoices`)
      .then((r) => r.ok ? r.json() : [])
      .then((data) => setInvoices(data))
      .catch(() => setInvoices([]))
      .finally(() => setLoading(false));
  }, []);

  return (
    <GenericListPage title="Invoices" eyebrow="Finance">
      {loading ? (
        <div className="flex justify-center py-10">
          <div className="h-8 w-8 animate-spin rounded-full border-4 border-accent border-t-transparent" />
        </div>
      ) : invoices.length === 0 ? (
        <p className="text-sm text-primary/50">No invoices found.</p>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {invoices.map((inv) => (
            <div key={inv.id} className="rounded border border-primary/10 bg-white p-4 shadow-sm">
              <div className="flex items-center justify-between">
                <h3 className="font-semibold">{inv.invoiceNumber}</h3>
                <StatusBadge status={inv.status} />
              </div>
              <p className="mt-1 text-sm text-primary/50">{inv.guestName}</p>
              <p className="mt-2 text-lg font-bold text-accent">${inv.totalAmount.toFixed(2)}</p>
            </div>
          ))}
        </div>
      )}
    </GenericListPage>
  );
}
