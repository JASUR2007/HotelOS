import { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

type PaymentMethod = 'Card' | 'Cash' | 'Online';

export default function PaymentPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const state = location.state as {
    bookingId?: number;
    roomId?: number;
    roomNumber?: string;
    guestName?: string;
    total?: number;
    nights?: number;
  } | null;

  const [method, setMethod] = useState<PaymentMethod>('Card');
  const [cardNumber, setCardNumber] = useState('');
  const [cardExpiry, setCardExpiry] = useState('');
  const [cardCvc, setCardCvc] = useState('');
  const [status, setStatus] = useState('');
  const [error, setError] = useState('');

  const total = state?.total || 0;
  const bookingId = state?.bookingId;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError('');
    setStatus('');

    try {
      const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
      const response = await fetch(`${apiBaseUrl}/payments/process`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          invoiceId: bookingId ?? 1,
          amount: total,
          method,
        }),
      });

      if (!response.ok) {
        const body = await response.json().catch(() => null);
        throw new Error(body?.message ?? 'Payment failed');
      }

      setStatus('Payment processed successfully!');
      setTimeout(() => navigate('/profile'), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Payment failed');
    }
  }

  return (
    <section className="container mx-auto max-w-xl px-4 py-28">
      <div className="mb-8 text-center">
        <p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
        <h1 className="h2 mt-3">Payment</h1>
        <p className="mt-2 text-sm text-primary/50">
          {bookingId ? `Booking #${bookingId}` : 'Complete your payment'}
        </p>
      </div>

      {state && (
        <div className="border border-primary/10 bg-white p-6 mb-6 shadow-sm">
          <h3 className="font-semibold mb-3">Booking Summary</h3>
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-primary/50">Room</span>
              <span>{state.roomNumber}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-primary/50">Guest</span>
              <span>{state.guestName}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-primary/50">Nights</span>
              <span>{state.nights}</span>
            </div>
            <div className="flex justify-between font-bold text-base pt-2 border-t border-primary/10">
              <span>Total</span>
              <span>${total.toLocaleString()}</span>
            </div>
          </div>
        </div>
      )}

      <form onSubmit={handleSubmit} className="border border-primary/10 bg-white p-8 shadow-sm">
        <div className="mb-6">
          <p className="mb-3 text-sm uppercase tracking-[0.25em] text-primary/50">Payment Method</p>
          <div className="flex gap-3">
            {(['Card', 'Cash', 'Online'] as PaymentMethod[]).map((m) => (
              <button
                key={m}
                type="button"
                onClick={() => setMethod(m)}
                className={`flex-1 border px-4 py-3 text-sm transition ${
                  method === m
                    ? 'border-accent bg-accent text-white'
                    : 'border-primary/10 text-primary/70 hover:border-accent'
                }`}
              >
                {m}
              </button>
            ))}
          </div>
        </div>

        {method === 'Card' && (
          <>
            <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
              Card Number
              <input
                className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
                value={cardNumber}
                onChange={(e) => setCardNumber(e.target.value)}
                placeholder="4242 4242 4242 4242"
                maxLength={19}
                required
              />
            </label>
            <div className="mt-5 grid grid-cols-2 gap-4">
              <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
                Expiry
                <input
                  className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
                  value={cardExpiry}
                  onChange={(e) => setCardExpiry(e.target.value)}
                  placeholder="MM/YY"
                  maxLength={5}
                  required
                />
              </label>
              <label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
                CVC
                <input
                  className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
                  value={cardCvc}
                  onChange={(e) => setCardCvc(e.target.value)}
                  placeholder="123"
                  maxLength={4}
                  required
                />
              </label>
            </div>
          </>
        )}

        {method === 'Cash' && (
          <p className="mt-2 rounded border border-amber-200 bg-amber-50 p-4 text-sm text-amber-700">
            Pay at the front desk during check-in. Your booking will be held for 24 hours.
          </p>
        )}

        {method === 'Online' && (
          <p className="mt-2 rounded border border-blue-200 bg-blue-50 p-4 text-sm text-blue-700">
            You will be redirected to our secure payment partner to complete the transaction.
          </p>
        )}

        <button type="submit" className="btn btn-primary btn-lg mt-6 w-full">
          {method === 'Cash' ? 'Confirm Cash Payment' : `Pay $${total.toLocaleString()} with ${method}`}
        </button>
        {error && <p className="mt-4 text-center text-sm text-red-600">{error}</p>}
        {status && <p className="mt-4 text-center text-sm text-emerald-700">{status}</p>}
      </form>
    </section>
  );
}
