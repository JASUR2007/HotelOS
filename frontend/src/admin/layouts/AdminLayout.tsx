import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';

export default function AdminLayout() {
  return (
    <div className="flex min-h-screen bg-[#f5efe7] text-primary">
      <Sidebar />
      <div className={`flex flex-1 flex-col transition-all`}>
        <Header />
        <main className="flex-1">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
