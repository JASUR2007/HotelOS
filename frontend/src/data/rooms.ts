import {
  Wifi,
  Coffee,
  Bath,
  Parking,
  Pool,
  Breakfast,
  Gym,
  Drinks,
} from '../components/SimpleIcons';
import type { Room } from '../types';
import images from '../assets';

// Shared facility list (name + icon) used for every room; icons from SimpleIcons.
const facilities = [
  { name: 'Wifi', icon: Wifi },
  { name: 'Coffee', icon: Coffee },
  { name: 'Bath', icon: Bath },
  { name: 'Parking Space', icon: Parking },
  { name: 'Swimming Pool', icon: Pool },
  { name: 'Breakfast', icon: Breakfast },
  { name: 'GYM', icon: Gym },
  { name: 'Drinks', icon: Drinks },
];

const description =
  'Lorem ipsum dolor sit amet consectetur adipisicing elit.Ea placeat eos sed voluptas unde veniam eligendi a. Quaerat molestiae hic omnis temporibus quos consequuntur nam voluptatum ea accusamus, corrupti nostrum eum placeat quibusdam quis beatae quae labore earum architecto aliquid debitis.';

// Static room list: used as initial state in RoomContext and filtered by handleCheck (total <= maxPerson).
export const roomData: Room[] = [
  {
    id: 1,
    name: 'Superior Room',
    description,
    facilities: [...facilities],
    size: 30,
    maxPerson: 1,
    price: 115,
    image: images.Room1Img,
    imageLg: images.Room1ImgLg,
  },
  {
    id: 2,
    name: 'Signature Room',
    description,
    facilities: [...facilities],
    size: 70,
    maxPerson: 2,
    price: 220,
    image: images.Room2Img,
    imageLg: images.Room2ImgLg,
  },
  {
    id: 3,
    name: 'Deluxe Room',
    description,
    facilities: [...facilities],
    size: 50,
    maxPerson: 3,
    price: 265,
    image: images.Room3Img,
    imageLg: images.Room3ImgLg,
  },
  {
    id: 4,
    name: 'Luxury Room',
    description,
    facilities: [...facilities],
    size: 50,
    maxPerson: 4,
    price: 289,
    image: images.Room4Img,
    imageLg: images.Room4ImgLg,
  },
  {
    id: 5,
    name: 'Luxury Suite Room',
    description,
    facilities: [...facilities],
    size: 90,
    maxPerson: 5,
    price: 320,
    image: images.Room5Img,
    imageLg: images.Room5ImgLg,
  },
  {
    id: 6,
    name: 'Deluxe Room',
    description,
    facilities: [...facilities],
    size: 45,
    maxPerson: 6,
    price: 344,
    image: images.Room6Img,
    imageLg: images.Room6ImgLg,
  },
  {
    id: 7,
    name: 'Luxury Room',
    description:
      'Lorem ipsum dolor sit amet consectetur adipisicing elit.Ea placeat eos sed voluptas unde veniam eligendi a. Quaerat molestiae hic omnis temporibus quos consequuntur nam voluptatum ea ccusamus, corrupti nostrum eum placeat quibusdam quis beatae quae labore earum architecto aliquid debitis.',
    facilities: [...facilities],
    size: 84,
    maxPerson: 7,
    price: 389,
    image: images.Room7Img,
    imageLg: images.Room7ImgLg,
  },
  {
    id: 8,
    name: 'Deluxe Room',
    description,
    facilities: [...facilities],
    size: 48,
    maxPerson: 8,
    price: 499,
    image: images.Room8Img,
    imageLg: images.Room8ImgLg,
  },
];
