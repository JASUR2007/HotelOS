import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';

export default function Checkout() {
  const navigate = useNavigate();
  const location = useLocation();
  const user = useAuthStore((state) => state.user);

  const state = location.state as {
    roomId?: number;
    roomNumber?: string;
    checkIn?: string;
    checkOut?: string;
    guests?: number;
    pricePerNight?: number;
    total?: number;
  } | null;

  const [roomNumber, setRoomNumber] = useState(state?.roomNumber || '');
  const [checkIn, setCheckIn] = useState(state?.checkIn || '');
  const [checkOut, setCheckOut] = useState(state?.checkOut || '');
  const [guests, setGuests] = useState(state?.guests || 1);
  const [guestName, setGuestName] = useState(user?.displayName || '');
  const [guestEmail, setGuestEmail] = useState(user?.email || '');
  const [status, setStatus] = useState('');
  const [error, setError] = useState('');

  const nights = (() => {
    if (!checkIn || !checkOut) return 0;
    const diff = Math.ceil((new Date(checkOut).getTime() - new Date(checkIn).getTime()) / (1000 * 60 * 60 * 24));
    return diff > 0 ? diff : 0;
  })();

  const pricePerNight = state?.pricePerNight || 220;
  const roomTotal = nights * pricePerNight;
  const taxes = Math.round(roomTotal * 0.05);
  const grandTotal = roomTotal + taxes;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError('');
    setStatus('');

    if (!user) {
      setError('Please log in to make a booking');
      return;
    }

    try {
      const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';

      const res = await fetch(`${apiBaseUrl}/reception/check-in`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          guestName: guestName || user.displayName,
          email: guestEmail || user.email,
          adults: guests,
          kids: 0,
          checkInDate: checkIn,
          checkOutDate: checkOut,
        }),
      });

      if (!res.ok) {
        const body = await res.json().catch(() => null);
        throw new Error(body?.message ?? 'Booking failed');
      }

      const data = await res.json();
      setStatus('Booking confirmed! Redirecting to payment...');
      setTimeout(() => navigate('/payment', {
        state: {
          bookingId: data.bookingId,
          roomId: data.roomId,
          roomNumber: roomNumber,
          guestName: guestName || user.displayName,
          total: grandTotal,
          nights,
        },
      }), 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Booking failed');
    }
  }

  return (
    <section className="container mx-auto max-w-xl px-4 py-28">
      <div className="mb-8 text-center">
        <p className="text-sm uppercase tracking-[0.35em] text-accent">GrandStay Hotel</p>
        <h1 className="h2 mt-3">Complete Your Booking</h1>
        <p className="mt-2 text-sm text-primary/70">Review and confirm your reservation details.</p>
      </div>

      <form onSubmit={handleSubmit} className="border border-primary/10 bg-white p-8 text-primary shadow-sm">
        <div className="grid grid-cols-2 gap-4 mb-5">
          <label className="block text-sm uppercase tracking-[0.25em] text-primary/80">
            Full Name
            <input
              className="mt-2 w-full border border-primary/10 px-4 py-3 text-primary outline-none focus:border-accent"
              value={guestName}
              onChange={(e) => setGuestName(e.target.value)}
              required
            />
          </label>
          <label className="block text-sm uppercase tracking-[0.25em] text-primary/80">
            Email
            <input
              className="mt-2 w-full border border-primary/10 px-4 py-3 text-primary outline-none focus:border-accent"
              type="email"
              value={guestEmail}
              onChange={(e) => setGuestEmail(e.target.value)}
              required
            />
          </label>
        </div>

        <label className="block text-sm uppercase tracking-[0.25em] text-primary/80">
          Room Number
          <input
            className="mt-2 w-full border border-primary/10 px-4 py-3 text-primary outline-none focus:border-accent"
            value={roomNumber}
            onChange={(e) => setRoomNumber(e.target.value)}
            placeholder="e.g. 101"
            required
          />
        </label>

        <div className="mt-5 grid grid-cols-2 gap-4">
          <label className="block text-sm uppercase tracking-[0.25em] text-primary/80">
            Check In
            <input
              className="mt-2 w-full border border-primary/10 px-4 py-3 text-primary outline-none focus:border-accent"
              type="date"
              value={checkIn}
              onChange={(e) => setCheckIn(e.target.value)}
              required
            />
          </label>
          <label className="block text-sm uppercase tracking-[0.25em] text-primary/80">
            Check Out
            <input
              className="mt-2 w-full border border-primary/10 px-4 py-3 text-primary outline-none focus:border-accent"
              type="date"
              value={checkOut}
              onChange={(e) => setCheckOut(e.target.value)}
              required
            />
          </label>
        </div>

        <label className="mt-5 block text-sm uppercase tracking-[0.25em] text-primary/80">
          Guests
          <select
            className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
            value={guests}
            onChange={(e) => setGuests(Number(e.target.value))}
          >
            {[1, 2, 3, 4, 5, 6].map((n) => (
              <option key={n} value={n}>{n} {n === 1 ? 'Guest' : 'Guests'}</option>
            ))}
          </select>
        </label>

        {nights > 0 && (
          <div className="mt-6 border-t border-primary/10 pt-6 space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-primary/50">${pricePerNight} × {nights} nights</span>
              <span>${roomTotal.toLocaleString()}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-primary/50">Taxes & fees (5%)</span>
              <span>${taxes}</span>
            </div>
            <div className="flex justify-between font-bold text-base pt-2 border-t border-primary/10">
              <span>Total</span>
              <span>${grandTotal.toLocaleString()}</span>
            </div>
          </div>
        )}

        {!user && (
          <p className="mt-4 text-center text-sm text-amber-600">
            Please <a href="/login" className="underline">log in</a> to complete your booking.
          </p>
        )}

        <button type="submit" className="btn btn-primary btn-lg mt-6 w-full" disabled={!user}>
          Confirm Booking - ${grandTotal.toLocaleString()}
        </button>
        {error && <p className="mt-4 text-center text-sm text-red-600">{error}</p>}
        {status && <p className="mt-4 text-center text-sm text-emerald-700">{status}</p>}
      </form>
    </section>
  );
}
