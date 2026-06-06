import { useRoomContext } from '../context/RoomContext';
import { SpinnerDotted } from 'spinners-react';
import Room from './Room';
import { useEffect, useMemo, useState } from 'react';
import { fetchBookingsAdmin } from '../api';

/**
 * Room grid with loading overlay. Lists rooms from RoomContext (filtered by capacity when user clicks "Check Now").
 * When loading is true, a full-screen overlay with SpinnerDotted is shown; grid uses responsive cols (1 on mobile, 3 on lg).
 */
type RoomsProps = {
  showRecommended?: boolean;
  showAllRooms?: boolean;
};

export default function Rooms({ showRecommended = true, showAllRooms = true }: RoomsProps) {
  const { rooms, loading } = useRoomContext();
  const [bookingCounts, setBookingCounts] = useState<Record<string, number>>({});

  useEffect(() => {
    fetchBookingsAdmin()
      .then((bookings) => {
        const counts: Record<string, number> = {};
        bookings.forEach((booking) => {
          counts[booking.roomNumber] = (counts[booking.roomNumber] ?? 0) + 1;
        });
        setBookingCounts(counts);
      })
      .catch(() => setBookingCounts({}));
  }, []);

  const recommendedRooms = useMemo(() => (
    [...rooms]
      .sort((a, b) => (bookingCounts[b.roomNumber] ?? 0) - (bookingCounts[a.roomNumber] ?? 0))
      .slice(0, 3)
  ), [bookingCounts, rooms]);

  return (
    <section className="py-24">
      {loading && (
        <div className="h-screen w-full fixed bottom-0 top-0 bg-black/80 z-50 grid place-items-center">
          <SpinnerDotted />
        </div>
      )}
      <div className="container mx-auto max-w-7xl lg:px-0">
        <div className="text-center">
          <p className="font-tertiary uppercase text-[15px] tracking-[6px]">GrandStay Hotel</p>
          <h2 className="font-primary text-[45px] mb-6">Room & Suites</h2>
        </div>
        {showRecommended && recommendedRooms.length > 0 && (
          <div className="mb-16">
            <div className="mb-8 text-center">
              <p className="font-tertiary uppercase text-[13px] tracking-[5px] text-accent">Most booked</p>
              <h3 className="font-primary text-[34px]">Recommended rooms</h3>
            </div>
            <div className="grid grid-cols-1 max-w-sm mx-auto gap-[30px] lg:grid-cols-3 lg:max-w-none lg:mx-0">
              {recommendedRooms.map((room) => (
                <Room key={`recommended-${room.id}`} room={room} />
              ))}
            </div>
          </div>
        )}
        {showAllRooms && (
          <div className="grid grid-cols-1 max-w-sm mx-auto gap-[30px] lg:grid-cols-3 lg:max-w-none lg:mx-0">
            {rooms.map((room) => (
              <Room key={room.id} room={room} />
            ))}
          </div>
        )}
      </div>
    </section>
  );
}
