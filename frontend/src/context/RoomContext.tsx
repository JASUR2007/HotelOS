import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import type { RoomContextValue, ApiRoom } from '../types';
import { fetchRooms } from '../api';

const RoomInfo = createContext<RoomContextValue | null>(null);

export function RoomContext({ children }: { children: ReactNode }) {
  const [rooms, setRooms] = useState<ApiRoom[]>([]);
  const [allRooms, setAllRooms] = useState<ApiRoom[]>([]);
  const [loading, setLoading] = useState(false);
  const [adults, setAdults] = useState('1 Adult');
  const [kids, setKids] = useState('0 Kid');
  const [total, setTotal] = useState(0);

  useEffect(() => {
    setTotal(+adults[0] + +kids[0]);
  }, [adults, kids]);

  useEffect(() => {
    fetchRooms()
      .then(data => { setRooms(data); setAllRooms(data); })
      .catch(() => {});
  }, []);

  const resetRoomFilterData = () => {
    setAdults('1 Adult');
    setKids('0 Kid');
    setRooms(allRooms);
  };

  const handleCheck = (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    const filterRooms = allRooms.filter((room) => total <= room.guestCapacity);
    setTimeout(() => {
      setLoading(false);
      setRooms(filterRooms);
    }, 3000);
  };

  const value: RoomContextValue = {
    rooms,
    loading,
    adults,
    setAdults,
    kids,
    setKids,
    total,
    handleCheck,
    resetRoomFilterData,
  };

  return <RoomInfo.Provider value={value}>{children}</RoomInfo.Provider>;
}

export function useRoomContext(): RoomContextValue {
  const ctx = useContext(RoomInfo);
  if (!ctx) throw new Error('useRoomContext must be used within RoomContext');
  return ctx;
}
