import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { ApiRoom } from '../types/room';

interface FavoritesState {
	favorites: ApiRoom[];
	addFavorite: (room: ApiRoom) => void;
	removeFavorite: (id: number) => void;
	isFavorite: (id: number) => boolean;
}

export const useFavoritesStore = create<FavoritesState>()(
	persist(
		(set, get) => ({
			favorites: [],
			addFavorite: (room) => {
				const exists = get().favorites.some((r) => r.id === room.id);
				if (!exists) {
					set({ favorites: [...get().favorites, room] });
				}
			},
			removeFavorite: (id) => {
				set({ favorites: get().favorites.filter((r) => r.id !== id) });
			},
			isFavorite: (id) => {
				return get().favorites.some((r) => r.id === id);
			},
		}),
		{ name: 'hotelos-favorites' },
	),
);
