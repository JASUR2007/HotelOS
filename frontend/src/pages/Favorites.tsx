import { Link } from 'react-router-dom';
import { useFavoritesStore } from '../store/favoritesStore';

export default function Favorites() {
	const favorites = useFavoritesStore((state) => state.favorites);
	const removeFavorite = useFavoritesStore((state) => state.removeFavorite);

	if (favorites.length === 0) {
		return (
			<section className="container mx-auto max-w-4xl px-4 py-28 text-center">
				<p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
				<h1 className="h2 mt-3">Your Favorites</h1>
				<p className="mt-6 text-primary/50">You have no saved rooms yet.</p>
				<Link to="/rooms" className="btn btn-primary mt-6 inline-block">
					Browse Rooms
				</Link>
			</section>
		);
	}

	return (
		<section className="container mx-auto max-w-7xl px-4 py-28">
			<div className="mb-8 text-center">
				<p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
				<h1 className="h2 mt-3">Your Favorites</h1>
			</div>
			<div className="grid gap-8 sm:grid-cols-2 lg:grid-cols-3">
				{favorites.map((room) => (
					<div key={room.id} className="group overflow-hidden border border-primary/10 bg-white shadow-sm">
						<div className="relative h-64 overflow-hidden">
							<Link to={`/room/${room.id}`}>
								<img src={room.mainImage} alt={room.roomNumber} className="h-full w-full object-cover transition duration-500 group-hover:scale-110" />
							</Link>
							<button
								onClick={() => removeFavorite(room.id)}
								className="absolute right-3 top-3 flex h-8 w-8 items-center justify-center rounded-full bg-white/80 text-red-500 shadow transition hover:bg-white hover:text-red-600"
								aria-label="Remove from favorites"
							>
								&times;
							</button>
						</div>
						<div className="p-6">
							<Link to={`/room/${room.id}`}>
								<h3 className="h3 hover:text-accent transition">Room {room.roomNumber}</h3>
							</Link>
							<p className="mt-2 text-sm text-primary/50">
								{room.type} &middot; Up to {room.guestCapacity} {room.guestCapacity === 1 ? 'Guest' : 'Guests'} &middot; ${room.pricePerNight} / night
							</p>
						</div>
					</div>
				))}
			</div>
		</section>
	);
}
