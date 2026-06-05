import type { ApiRoom } from './room';

export interface RoomContextValue {
  rooms: ApiRoom[];
  loading: boolean;
  adults: string;
  setAdults: (value: string) => void;
  kids: string;
  setKids: (value: string) => void;
  handleCheck: (e: React.FormEvent) => void;
  resetRoomFilterData: () => void;
}
