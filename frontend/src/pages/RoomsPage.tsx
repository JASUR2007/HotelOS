import { useState, useEffect, useMemo } from 'react';
import { Link } from 'react-router-dom';
import { useRoomContext } from '../context/RoomContext';
import { useFavoritesStore } from '../store/favoritesStore';
import { fetchBookingsAdmin } from '../api';

export default function RoomsPage() {
	const { rooms, loading, adults, kids, setAdults, setKids, total } = useRoomContext();
	const [search, setSearch] = useState('');
	const [checkIn, setCheckIn] = useState('');
	const [checkOut, setCheckOut] = useState('');
	const [allBookings, setAllBookings] = useState<any[]>([]);
	const favorites = useFavoritesStore((s) => s.favorites);
	const addFavorite = useFavoritesStore((s) => s.addFavorite);
	const removeFavorite = useFavoritesStore((s) => s.removeFavorite);
	const isFavorite = (id: number) => favorites.some((room) => room.id === id);

	useEffect(() => {
		fetchBookingsAdmin().then(setAllBookings).catch(() => {});
	}, []);

	const bookOverlap = useMemo(() => {
		const set = new Set<string>();
		if (!checkIn || !checkOut) return set;
		const ci = new Date(`${checkIn}T00:00:00`);
		const co = new Date(`${checkOut}T00:00:00`);
		if (Number.isNaN(ci.getTime()) || Number.isNaN(co.getTime())) return set;
		for (const b of allBookings) {
			if (b.status === 'Cancelled' || b.status === 'CheckedOut') continue;
			const bci = new Date(`${b.checkInDate}T00:00:00`);
			const bco = new Date(`${b.checkOutDate}T00:00:00`);
			if (Number.isNaN(bci.getTime()) || Number.isNaN(bco.getTime())) continue;
			if (bci < co && bco > ci) set.add(String(b.roomNumber));
		}
		return set;
	}, [allBookings, checkIn, checkOut]);

	const allActiveBookings = useMemo(() => {
		const set = new Set<string>();
		for (const b of allBookings) {
			if (b.status === 'Cancelled' || b.status === 'CheckedOut') continue;
			set.add(String(b.roomNumber));
		}
		return set;
	}, [allBookings]);

	const filtered = rooms.filter((r) => {
		const isNumeric = /^\d+$/.test(search.trim());
		const matchesSearch = !search.trim() || r.roomNumber.includes(search) || (r as any).type?.toLowerCase().includes(search.toLowerCase());
		const matchesGuests = total <= r.guestCapacity;
		const isAvailable = r.status === 'Available';
		const dateConflict = bookOverlap.has(String(r.roomNumber));
		const anyConflict = isNumeric && allActiveBookings.has(String(r.roomNumber));
		return matchesSearch && matchesGuests && isAvailable && !dateConflict && !anyConflict;
	});

	return (
		<section className="container mx-auto max-w-7xl px-4 py-28">
			<div className="mb-8 text-center">
				<p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
				<h1 className="h2 mt-3">Our Rooms</h1>
				<p className="mt-2 text-sm text-primary/50">Browse available rooms and find your perfect stay.</p>
			</div>

			<div className="mb-10 flex flex-wrap items-end justify-center gap-4">
				<div className="flex flex-col gap-1">
					<label className="text-xs uppercase tracking-widest text-primary/50">Check-in</label>
					<input
						type="date"
						value={checkIn}
						onChange={(e) => setCheckIn(e.target.value)}
						className="border border-primary/10 px-4 py-2 text-sm outline-none focus:border-accent w-44"
					/>
				</div>
				<div className="flex flex-col gap-1">
					<label className="text-xs uppercase tracking-widest text-primary/50">Check-out</label>
					<input
						type="date"
						value={checkOut}
						onChange={(e) => setCheckOut(e.target.value)}
						className="border border-primary/10 px-4 py-2 text-sm outline-none focus:border-accent w-44"
					/>
				</div>
				<div className="flex flex-col gap-1">
					<label className="text-xs uppercase tracking-widest text-primary/50">Search</label>
					<input
						type="text"
						placeholder="Room number or type..."
						value={search}
						onChange={(e) => setSearch(e.target.value)}
						className="border border-primary/10 px-4 py-2 text-sm outline-none focus:border-accent w-44"
					/>
				</div>
				<div className="flex flex-col gap-1">
					<label className="text-xs uppercase tracking-widest text-primary/50">Adults</label>
					<select
						className="border border-primary/10 px-3 py-2 text-sm outline-none focus:border-accent"
						value={adults}
						onChange={(e) => setAdults(e.target.value)}
					>
						{['1 Adult', '2 Adults', '3 Adults', '4 Adults'].map((v) => (
							<option key={v} value={v}>{v}</option>
						))}
					</select>
				</div>
				<div className="flex flex-col gap-1">
					<label className="text-xs uppercase tracking-widest text-primary/50">Kids</label>
					<select
						className="border border-primary/10 px-3 py-2 text-sm outline-none focus:border-accent"
						value={kids}
						onChange={(e) => setKids(e.target.value)}
					>
						{['0 Kids', '1 Kid', '2 Kids', '3 Kids'].map((v) => (
							<option key={v} value={v}>{v}</option>
						))}
					</select>
				</div>
				<button type="button" onClick={() => { setSearch(''); setCheckIn(''); setCheckOut(''); }} className="rounded-lg border border-primary/10 px-4 py-2 text-sm text-primary/60 hover:bg-primary/5">
					Clear
				</button>
			</div>

			{loading ? (
				<div className="flex justify-center py-20">
					<div className="h-10 w-10 animate-spin rounded-full border-4 border-accent border-t-transparent" />
				</div>
			) : filtered.length === 0 ? (
				<p className="py-20 text-center text-primary/50">No rooms match your criteria.</p>
			) : (
				<div className="grid gap-8 sm:grid-cols-2 lg:grid-cols-3">
					{filtered.map((room) => (
						<div
							key={room.id}
							className="group overflow-hidden rounded-2xl border border-primary/10 bg-white shadow-sm transition hover:shadow-md"
						>
														<div className="relative h-64 overflow-hidden">
								<img
									src={room.mainImage}
									alt={room.roomNumber}
									className="h-full w-full object-cover transition duration-500 group-hover:scale-110"
								/>
								<div className="absolute right-4 top-4 rounded-full bg-accent px-3 py-1 text-xs font-semibold uppercase text-white">
									${room.pricePerNight}
								</div>
								<button
									type="button"
									onClick={(e) => {
										e.preventDefault();
										e.stopPropagation();
										isFavorite(room.id) ? removeFavorite(room.id) : addFavorite(room);
									}}
									className="absolute left-4 top-4 flex h-9 w-9 items-center justify-center rounded-full bg-white/80 shadow transition hover:bg-white"
									aria-label={isFavorite(room.id) ? 'Remove from favorites' : 'Add to favorites'}
								>
									<svg className={`h-5 w-5 ${isFavorite(room.id) ? 'text-red-500' : 'text-gray-400'}`} fill={isFavorite(room.id) ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
										<path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" />
									</svg>
								</button>
							</div>
							<Link to={`/room/${room.id}`} className="block text-inherit">
								<div className="p-6">
								<h3 className="h3">Room {room.roomNumber}</h3>
								<p className="mt-2 text-sm text-primary/50">
									Up to {room.guestCapacity} {room.guestCapacity === 1 ? 'Guest' : 'Guests'}
								</p>
								<p className="mt-3 text-xs uppercase tracking-widest text-accent">
									View Details &rarr;
								</p>
						</div>
					</Link>
				</div>
					))}
				</div>
			)}
		</section>
	);
}
