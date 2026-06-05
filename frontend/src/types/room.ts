import type { ElementType } from 'react';

export interface Facility {
  name: string;
  icon: ElementType;
}

export interface Room {
  id: number;
  name: string;
  description: string;
  facilities: Facility[];
  size: number;
  maxPerson: number;
  price: number;
  image: string;
  imageLg: string;
}

export interface ApiRoom {
  id: number;
  roomNumber: string;
  type: string;
  status: string;
  pricePerNight: number;
  floor: number;
  description: string;
  guestCapacity: number;
  mainImage: string;
  images: string[];
  amenities: string[];
}
