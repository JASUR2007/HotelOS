import { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

type PaymentMethod = 'Card' | 'Cash' | 'Online';

const TIMER_MINUTES = 10;

export default function PaymentPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const state = location.state as {
    invoiceId?: number;
    bookingId?: number;
    roomId?: number;
    roomNumber?: string;
    guestName?: string;
    total?: number;
    nights?: number;
    roomNightsTotal?: number;
    foodOrdersTotal?: number;
    minibarTotal?: number;
    damagesTotal?: number;
    discountsTotal?: number;
  } | null;

  const [method, setMethod] = useState<PaymentMethod>('Card');
  const [cardNumber, setCardNumber] = useState('');
  const [cardExpiry, setCardExpiry] = useState('');
  const [cardCvc, setCardCvc] = useState('');
  const [status, setStatus] = useState('');
  const [error, setError] = useState('');
  const [timeLeft, setTimeLeft] = useState(TIMER_MINUTES * 60);

  const total = state?.total || 0;
  const invoiceId = state?.invoiceId ?? null;
  const bookingId = state?.bookingId;
  const roomNightsTotal = state?.roomNightsTotal || 0;
  const foodOrdersTotal = state?.foodOrdersTotal || 0;
  const minibarTotal = state?.minibarTotal || 0;
  const damagesTotal = state?.damagesTotal || 0;
  const discountsTotal = state?.discountsTotal || 0;

  useEffect(() => {
    if (timeLeft <= 0) return;
    const timer = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          clearInterval(timer);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
    return () => clearInterval(timer);
  }, []);

  const minutes = Math.floor(timeLeft / 60);
  const seconds = timeLeft % 60;
  const isExpired = timeLeft <= 0;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (isExpired) {
      setError('Payment time has expired. Please start a new booking.');
      return;
    }

    if (!invoiceId) {
      setError('We could not find your invoice. Please return to checkout and try again.');
      return;
    }

    if (method === 'Card' && (!cardNumber.trim() || !cardExpiry.trim() || !cardCvc.trim())) {
      setError('Please complete all card fields before submitting payment.');
      return;
    }

    setError('');
    setStatus('');

    try {
      const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
      const response = await fetch(`${apiBaseUrl}/payments/process`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          invoiceId,
          amount: total,
          method,
          cardNumber: method === 'Card' ? cardNumber.trim() : undefined,
          cardExpiry: method === 'Card' ? cardExpiry.trim() : undefined,
          cardCvc: method === 'Card' ? cardCvc.trim() : undefined,
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

      <div className={`border p-6 mb-6 text-center shadow-sm ${isExpired ? 'border-red-300 bg-red-50' : 'border-amber-200 bg-amber-50'}`}>
        <p className="text-xs uppercase tracking-[0.35em] text-accent mb-1">Payment Timer</p>
        {isExpired ? (
          <p className="text-lg font-bold text-red-600">Time Expired</p>
        ) : (
          <p className="text-2xl font-bold text-amber-700">{String(minutes).padStart(2, '0')}:{String(seconds).padStart(2, '0')}</p>
        )}
        <p className="text-xs text-primary/50 mt-1">Your room is held for {TIMER_MINUTES} minutes while you complete payment.</p>
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
            {roomNightsTotal > 0 && (
              <>
                <div className="pt-2 border-t border-primary/10 space-y-2">
                  {roomNightsTotal > 0 && (
                    <div className="flex justify-between text-xs text-primary/40">
                      <span>Room nights</span>
                      <span>${roomNightsTotal.toLocaleString()}</span>
                    </div>
                  )}
                  {foodOrdersTotal > 0 && (
                    <div className="flex justify-between text-xs text-primary/40">
                      <span>Food & beverages</span>
                      <span>${foodOrdersTotal.toLocaleString()}</span>
                    </div>
                  )}
                  {minibarTotal > 0 && (
                    <div className="flex justify-between text-xs text-primary/40">
                      <span>Minibar</span>
                      <span>${minibarTotal.toLocaleString()}</span>
                    </div>
                  )}
                  {damagesTotal > 0 && (
                    <div className="flex justify-between text-xs text-primary/40">
                      <span>Damages</span>
                      <span>${damagesTotal.toLocaleString()}</span>
                    </div>
                  )}
                  {discountsTotal > 0 && (
                    <div className="flex justify-between text-xs text-primary/40">
                      <span>Discounts</span>
                      <span>-${discountsTotal.toLocaleString()}</span>
                    </div>
                  )}
                </div>
              </>
            )}
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
