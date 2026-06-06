import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ScrollToTop } from '../components';

import type { ApiRoom } from '../types';
import { fetchRoomById } from '../api';

function AmenityIcon({ name }: { name: string }) {
  const svg = AMENITY_SVG[name];
  if (!svg) return <span className="text-accent text-sm font-bold">✓</span>;
  return <svg className="w-5 h-5 text-accent" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>{svg}</svg>;
}

const AMENITY_SVG: Record<string, JSX.Element> = {
  'WiFi': <><path strokeLinecap="round" strokeLinejoin="round" d="M8.288 15.038a5.25 5.25 0 017.424 0M5.106 11.856c3.807-3.808 9.98-3.808 13.788 0M1.924 8.674c5.565-5.565 14.587-5.565 20.152 0" /></>,
  'Coffee Machine': <><path strokeLinecap="round" strokeLinejoin="round" d="M9.53 16.122a3 3 0 00-5.78 1.128 2.25 2.25 0 01-2.4 2.245 4.5 4.5 0 008.4-2.245c0-.399-.078-.78-.22-1.128zm0 0a15.998 15.998 0 003.388-1.62m-5.043-.025a15.994 15.994 0 011.622-3.395m3.42 3.42a15.995 15.995 0 004.764-4.648l3.876-5.814a1.151 1.151 0 00-1.597-1.597L14.146 6.32a15.996 15.996 0 00-4.649 4.763m3.42 3.42a6.776 6.776 0 00-3.42-3.42" /></>,
  'Air Conditioning': <><path strokeLinecap="round" strokeLinejoin="round" d="M9.813 15.904L9 18.75l-.813-2.846a4.5 4.5 0 00-3.09-3.09L2.25 12l2.846-.813a4.5 4.5 0 003.09-3.09L9 5.25l.813 2.846a4.5 4.5 0 003.09 3.09L15.75 12l-2.846.813a4.5 4.5 0 00-3.09 3.09zM18.259 8.715L18 9.75l-.259-1.035a3.375 3.375 0 00-2.455-2.456L14.25 6l1.036-.259a3.375 3.375 0 002.455-2.456L18 2.25l.259 1.035a3.375 3.375 0 002.455 2.456L21.75 6l-1.036.259a3.375 3.375 0 00-2.455 2.456zM16.894 20.567L16.5 21.75l-.394-1.183a2.25 2.25 0 00-1.423-1.423L13.5 18.75l1.183-.394a2.25 2.25 0 001.423-1.423l.394-1.183.394 1.183a2.25 2.25 0 001.423 1.423l1.183.394-1.183.394a2.25 2.25 0 00-1.423 1.423z" /></>,
  'Smart TV': <><path strokeLinecap="round" strokeLinejoin="round" d="M6 20.25h12m-7.5-3v3m3-3v3m-10.125-3h17.25c.621 0 1.125-.504 1.125-1.125V4.875c0-.621-.504-1.125-1.125-1.125H3.375c-.621 0-1.125.504-1.125 1.125v11.25c0 .621.504 1.125 1.125 1.125z" /></>,
  'TV': <><path strokeLinecap="round" strokeLinejoin="round" d="M6 20.25h12m-7.5-3v3m3-3v3m-10.125-3h17.25c.621 0 1.125-.504 1.125-1.125V4.875c0-.621-.504-1.125-1.125-1.125H3.375c-.621 0-1.125.504-1.125 1.125v11.25c0 .621.504 1.125 1.125 1.125z" /></>,
  'Mini Bar': <><path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" /></>,
  'Bath': <><path strokeLinecap="round" strokeLinejoin="round" d="M4.745 3A23.933 23.933 0 003 12c0 6.21 1.992 11.959 5.36 16.642M14.372 2.058A21.93 21.93 0 0112 12c0 4.42.82 8.639 2.372 12.558M9 12a3 3 0 11-6 0 3 3 0 016 0zM21 12a3 3 0 11-6 0 3 3 0 016 0z" /></>,
  'Jacuzzi': <><path strokeLinecap="round" strokeLinejoin="round" d="M12 3v2.25m6.364.386l-1.591 1.591M21 12h-2.25m-.386 6.364l-1.591-1.591M12 18.75V21m-4.773-4.227l-1.591 1.591M5.25 12H3m4.227-4.773L5.636 5.636M15.75 12a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0z" /></>,
  'Balcony': <><path strokeLinecap="round" strokeLinejoin="round" d="M2.25 21h19.5m-18-18v18m10.5-18v18m6-13.5V21M6.75 6.75h.75m-.75 3h.75m-.75 3h.75m3-6h.75m-.75 3h.75m-.75 3h.75M6.75 21v-3.375c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21M3 3h12m-.75 4.5H21m-3.75 3.75h.008v.008h-.008v-.008zm0 3h.008v.008h-.008v-.008zm0 3h.008v.008h-.008v-.008z" /></>,
  'Wheelchair Access': <><circle cx="12" cy="5" r="1.5" /><path strokeLinecap="round" strokeLinejoin="round" d="M18 11.25V15a3 3 0 01-3 3h-3m-4.5 0a3 3 0 01-3-3v-1.5m4.5 0a3 3 0 013 3v1.5m-3-7.5v3m0 0l3 3m-3-3l-3 3" /></>,
  'Roll-in Shower': <><path strokeLinecap="round" strokeLinejoin="round" d="M12 3v18m0 0l-3-3m3 3l3-3M3 12h18" /></>,
  'Butler Service': <><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" /></>,
};

export default function RoomDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [room, setRoom] = useState<ApiRoom | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeImage, setActiveImage] = useState(0);
  const [checkIn, setCheckIn] = useState('');
  const [checkOut, setCheckOut] = useState('');
  const [guests, setGuests] = useState(1);
  const [error, setError] = useState('');

  useEffect(() => {
    if (id) {
      setLoading(true);
      fetchRoomById(Number(id))
        .then(setRoom)
        .catch(() => setError('Failed to load room details'))
        .finally(() => setLoading(false));
    }
  }, [id]);

  if (loading) {
    return (
      <section>
        <ScrollToTop />
        <div className="container mx-auto max-w-7xl py-24 text-center">
          <div className="animate-pulse">
            <div className="h-[400px] bg-gray-200 rounded mb-8" />
            <div className="h-8 bg-gray-200 w-1/3 mx-auto mb-4 rounded" />
            <div className="h-4 bg-gray-200 w-1/2 mx-auto rounded" />
          </div>
        </div>
      </section>
    );
  }

  if (!room || error) {
    return (
      <section>
        <ScrollToTop />
        <div className="container mx-auto max-w-7xl py-24 text-center">
          <p className="text-lg text-primary/50">{error || 'Room not found.'}</p>
        </div>
      </section>
    );
  }

  const nights = (() => {
    if (!checkIn || !checkOut) return 0;
    const start = new Date(checkIn);
    const end = new Date(checkOut);
    const diff = Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
    return diff > 0 ? diff : 0;
  })();

  const roomPrice = nights * room.pricePerNight;
  const taxes = Math.round(roomPrice * 0.05);
  const total = roomPrice + taxes;

  const allImages = room.images?.length ? room.images : [room.mainImage];

  const handleBook = async () => {
    if (!checkIn || !checkOut) {
      setError('Please select check-in and check-out dates');
      return;
    }
    setError('');
    try {
      const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
      await fetch(`${apiBaseUrl}/reception/hold`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          roomId: room.id, roomNumber: room.roomNumber,
          checkInDate: checkIn, checkOutDate: checkOut, guestsCount: guests,
        }),
      });
    } catch { /* hold is best-effort */ }
    navigate('/checkout', {
      state: {
        roomId: room.id,
        roomNumber: room.roomNumber,
        checkIn,
        checkOut,
        guests,
        pricePerNight: room.pricePerNight,
        total,
      },
    });
  };

  return (
    <section>
      <ScrollToTop />
      <div
        className="h-[560px] relative flex justify-center items-center bg-cover bg-center"
        style={{ backgroundImage: `url(${allImages[activeImage]})` }}
      >
        <div className="absolute w-full h-full bg-black/50" />
        <div className="z-20 text-center">
          <p className="text-sm uppercase tracking-[0.35em] text-accent mb-2">HotelOS</p>
          <h1 className="text-5xl lg:text-6xl text-white font-primary">
            Room {room.roomNumber}
          </h1>
          <p className="text-white/80 mt-4 text-lg">{room.type} · Floor {room.floor}</p>
        </div>
      </div>

      <div className="container mx-auto max-w-7xl px-4">
        {/* Gallery Thumbnails */}
        {allImages.length > 1 && (
          <div className="flex gap-3 mt-6 overflow-x-auto pb-2">
            {allImages.map((img, idx) => (
              <button
                key={idx}
                onClick={() => setActiveImage(idx)}
                className={`flex-shrink-0 w-24 h-16 rounded overflow-hidden border-2 transition-all ${
                  idx === activeImage ? 'border-accent' : 'border-transparent opacity-60 hover:opacity-100'
                }`}
              >
                <img src={img} alt={`View ${idx + 1}`} className="w-full h-full object-cover" />
              </button>
            ))}
          </div>
        )}

        <div className="flex flex-col lg:flex-row lg:gap-x-12 py-12">
          {/* Left: Details */}
          <div className="flex-1">
            <div className="flex items-center gap-4 mb-4">
              <span className={`px-3 py-1 rounded-full text-xs font-semibold uppercase ${
                room.status === 'Available' ? 'bg-emerald-100 text-emerald-700' :
                room.status === 'Occupied' ? 'bg-red-100 text-red-700' :
                'bg-amber-100 text-amber-700'
              }`}>
                {room.status}
              </span>
              <span className="text-sm text-primary/50">Floor {room.floor} · Up to {room.guestCapacity} guests</span>
            </div>

            <h2 className="h2 mb-4">About This Room</h2>
            <p className="text-primary/70 leading-relaxed mb-8">{room.description}</p>

            {/* Amenities */}
            <h3 className="h3 mb-4">Amenities</h3>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-3 mb-8">
              {room.amenities?.map((amenity, idx) => (
                <div key={idx} className="flex items-center gap-x-3 p-3 bg-accent/5 rounded">
                  <AmenityIcon name={amenity} />
                  <span className="text-sm">{amenity}</span>
                </div>
              ))}
            </div>
          </div>

          {/* Right: Booking Card */}
          <div className="w-full lg:w-[380px]">
            <div className="sticky top-8 bg-white border border-primary/10 shadow-lg p-6">
              <div className="flex items-baseline justify-between mb-6">
                <span className="text-2xl font-bold">${room.pricePerNight}</span>
                <span className="text-primary/50 text-sm">per night</span>
              </div>

              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-3">
                  <label className="block">
                    <span className="text-xs uppercase tracking-wider text-primary/50">Check In</span>
                    <input
                      type="date"
                      min={new Date().toISOString().split('T')[0]}
                      className="mt-1 w-full border border-primary/10 px-3 py-2.5 text-sm outline-none focus:border-accent"
                      value={checkIn}
                      onChange={(e) => setCheckIn(e.target.value)}
                    />
                  </label>
                  <label className="block">
                    <span className="text-xs uppercase tracking-wider text-primary/50">Check Out</span>
                    <input
                      type="date"
                      min={checkIn || new Date().toISOString().split('T')[0]}
                      className="mt-1 w-full border border-primary/10 px-3 py-2.5 text-sm outline-none focus:border-accent"
                      value={checkOut}
                      onChange={(e) => setCheckOut(e.target.value)}
                    />
                  </label>
                </div>

                <label className="block">
                  <span className="text-xs uppercase tracking-wider text-primary/50">Guests</span>
                  <select
                    className="mt-1 w-full border border-primary/10 px-3 py-2.5 text-sm outline-none focus:border-accent"
                    value={guests}
                    onChange={(e) => setGuests(Number(e.target.value))}
                  >
                    {Array.from({ length: room.guestCapacity }, (_, i) => i + 1).map(n => (
                      <option key={n} value={n}>{n} {n === 1 ? 'Guest' : 'Guests'}</option>
                    ))}
                  </select>
                </label>

                {nights > 0 && (
                  <div className="border-t border-primary/10 pt-4 space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-primary/50">${room.pricePerNight} × {nights} nights</span>
                      <span>${roomPrice.toLocaleString()}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-primary/50">Taxes & fees</span>
                      <span>${taxes}</span>
                    </div>
                    <div className="flex justify-between font-bold text-base pt-2 border-t border-primary/10">
                      <span>Total</span>
                      <span>${total.toLocaleString()}</span>
                    </div>
                  </div>
                )}
              </div>

              {error && (
                <div className="mt-4 p-3 bg-red-50 border border-red-200 rounded text-sm text-red-700">
                  {error}
                </div>
              )}

              <div className="mt-6">
                <button
                  onClick={handleBook}
                  disabled={nights === 0}
                  className={`btn btn-lg w-full ${nights > 0 ? 'btn-primary' : 'opacity-50 cursor-not-allowed bg-gray-300 text-gray-500'}`}
                >
                  {nights > 0 ? `Book Now - $${total.toLocaleString()}` : 'Select dates to book'}
                </button>
                <p className="text-center text-xs text-primary/40 mt-3">Your room will be held for 10 minutes while you complete payment.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
