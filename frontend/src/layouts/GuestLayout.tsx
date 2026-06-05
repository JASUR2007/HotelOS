import { Outlet } from 'react-router-dom';
import { Header, Footer } from '../components';

export default function GuestLayout() {
  return (
    <main className="">
      <Header />
      <Outlet />
      <Footer />
    </main>
  );
}
