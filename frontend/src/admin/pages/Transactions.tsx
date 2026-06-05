import { useEffect, useState } from 'react';
import GenericListPage from './GenericListPage';
import { fetchTransactions } from '../../api';
import type { TransactionRecord } from '../../types';

export default function Transactions() {
  const [transactions, setTransactions] = useState<TransactionRecord[]>([]);

  useEffect(() => {
    fetchTransactions().then(setTransactions).catch(() => undefined);
  }, []);

  return (
    <GenericListPage eyebrow="Finance" title="Transactions monitoring">
      <div className="grid gap-4 lg:grid-cols-2">
        {transactions.map((transaction) => (
          <article key={transaction.id} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
            <div className="flex items-center justify-between gap-4">
              <h2 className="h3">{transaction.invoiceNumber}</h2>
              <span className="text-sm uppercase tracking-[0.22em] text-primary/50">{transaction.status}</span>
            </div>
            <p className="mt-2 text-sm text-primary/65">{transaction.guestName}</p>
            <p className="mt-2 text-sm text-primary/65">{transaction.paymentMethod}</p>
            <p className="mt-2 text-sm text-primary/65">${transaction.amount}</p>
          </article>
        ))}
      </div>
    </GenericListPage>
  );
}
