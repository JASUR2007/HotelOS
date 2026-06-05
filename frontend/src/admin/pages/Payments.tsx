import { useEffect, useState } from 'react';
import { fetchPaymentAnalytics, fetchPaymentStats, fetchTransactions } from '../../api';
import type { PaymentAnalyticsPoint, PaymentStats, TransactionRecord } from '../../types';
import StatusBadge from '../components/StatusBadge';

type Tab = 'overview' | 'invoices' | 'transactions';

interface Invoice {
  id: number;
  invoiceNumber: string;
  guestName: string;
  roomNumber: string;
  totalAmount: number;
  status: string;
  bookingId?: number;
}

export default function Payments() {
  const [tab, setTab] = useState<Tab>('overview');
  const [stats, setStats] = useState<PaymentStats[]>([]);
  const [analytics, setAnalytics] = useState<PaymentAnalyticsPoint[]>([]);
  const [transactions, setTransactions] = useState<TransactionRecord[]>([]);
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    Promise.all([
      fetchPaymentStats().then(setStats).catch(() => []),
      fetchPaymentAnalytics().then(setAnalytics).catch(() => []),
      fetchTransactions().then(setTransactions).catch(() => []),
      (async () => {
        const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
        const r = await fetch(`${apiBaseUrl}/payments/invoices`);
        return r.ok ? r.json() : [];
      })().then(setInvoices).catch(() => []),
    ]).finally(() => setLoading(false));
  }, []);

  const tabs: { key: Tab; label: string }[] = [
    { key: 'overview', label: 'Overview' },
    { key: 'invoices', label: `Invoices (${invoices.length})` },
    { key: 'transactions', label: `Transactions (${transactions.length})` },
  ];

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Finance</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Payments</h1>
      </div>

      <div className="mb-6 flex gap-1 rounded-xl border border-primary/10 bg-white p-1 w-fit">
        {tabs.map((t) => (
          <button
            key={t.key}
            onClick={() => setTab(t.key)}
            className={`rounded-lg px-4 py-2 text-sm font-medium transition-colors ${
              tab === t.key ? 'bg-accent text-white' : 'text-primary/60 hover:text-primary'
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {loading ? (
        <div className="flex justify-center py-10">
          <div className="h-8 w-8 animate-spin rounded-full border-4 border-accent border-t-transparent" />
        </div>
      ) : (
        <>
          {tab === 'overview' && (
            <>
              <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                {stats.map((item) => (
                  <div key={item.label} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
                    <p className="text-xs uppercase tracking-[0.28em] text-primary/40">{item.label}</p>
                    <div className="mt-2 text-3xl font-primary text-primary">{item.value}</div>
                    <p className="mt-2 text-sm text-primary/65">{item.delta}</p>
                  </div>
                ))}
              </div>
              <div className="mt-6 grid gap-4 md:grid-cols-4">
                {analytics.map((point) => (
                  <div key={point.label} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
                    <p className="text-xs uppercase tracking-[0.28em] text-primary/40">{point.label}</p>
                    <div className="mt-2 text-2xl font-primary text-primary">{point.value}</div>
                  </div>
                ))}
              </div>
            </>
          )}

          {tab === 'invoices' && (
            invoices.length === 0 ? (
              <p className="text-sm text-primary/50">No invoices found.</p>
            ) : (
              <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {invoices.map((inv) => (
                  <div key={inv.id} className="rounded border border-primary/10 bg-white p-4 shadow-sm">
                    <div className="flex items-center justify-between">
                      <h3 className="font-semibold">{inv.invoiceNumber}</h3>
                      <StatusBadge status={inv.status} />
                    </div>
                    <p className="mt-1 text-sm text-primary/50">{inv.guestName}{inv.roomNumber ? ` — Room ${inv.roomNumber}` : ''}</p>
                    <p className="mt-2 text-lg font-bold text-accent">${(inv.totalAmount ?? 0).toFixed(2)}</p>
                  </div>
                ))}
              </div>
            )
          )}

          {tab === 'transactions' && (
            transactions.length === 0 ? (
              <p className="text-sm text-primary/50">No transactions found.</p>
            ) : (
              <div className="grid gap-4 lg:grid-cols-2">
                {transactions.map((t) => (
                  <article key={t.id} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
                    <div className="flex items-center justify-between gap-4">
                      <h2 className="font-semibold">{t.invoiceNumber}</h2>
                      <span className="text-sm uppercase tracking-[0.22em] text-primary/50">{t.status}</span>
                    </div>
                    <p className="mt-2 text-sm text-primary/65">{t.guestName}{t.roomNumber ? ` — Room ${t.roomNumber}` : ''}</p>
                    <p className="mt-2 text-sm text-primary/65">{t.paymentMethod} — ${t.amount}</p>
                    <p className="mt-1 text-xs text-primary/30">{t.createdAt}</p>
                  </article>
                ))}
              </div>
            )
          )}
        </>
      )}
    </div>
  );
}
