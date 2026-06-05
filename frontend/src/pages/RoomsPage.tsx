import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useRoomContext } from '../context/RoomContext';

export default function RoomsPage() {
	const { rooms, loading, adults, kids, setAdults, setKids } = useRoomContext();
	const [search, setSearch] = useState('');

	const filtered = search.trim()
		? rooms.filter((r) => r.roomNumber.includes(search) || (r as any).type?.toLowerCase().includes(search.toLowerCase()))
		: rooms;

	return (
		<section className="container mx-auto max-w-7xl px-4 py-28">
			<div className="mb-8 text-center">
				<p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
				<h1 className="h2 mt-3">Our Rooms</h1>
				<p className="mt-2 text-sm text-primary/50">Browse available rooms and find your perfect stay.</p>
			</div>

			<div className="mb-10 flex flex-wrap items-end justify-center gap-4">
				<div className="flex flex-col gap-1">
					<label className="text-xs uppercase tracking-widest text-primary/50">Search</label>
					<input
						type="text"
						placeholder="Room number or type..."
						value={search}
						onChange={(e) => setSearch(e.target.value)}
						className="border border-primary/10 px-4 py-2 text-sm outline-none focus:border-accent w-52"
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
				<button type="button" onClick={() => setSearch('')} className="rounded-lg border border-primary/10 px-4 py-2 text-sm text-primary/60 hover:bg-primary/5">
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
						<Link
							key={room.id}
							to={`/room/${room.id}`}
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
							</div>
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
					))}
				</div>
			)}
		</section>
	);
}
