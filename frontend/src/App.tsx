import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import { PageNotFound } from './components';
import { Home, Register, RoomDetails, RoomsPage, Favorites, Checkout, PaymentPage, Profile } from './pages';
import GuestLayout from './layouts/GuestLayout';

import AdminLayout from './admin/layouts/AdminLayout';
import ProtectedRoute from './admin/routes/ProtectedRoute';
import {
  AdminDashboard,
  RoomsPage as AdminRoomsPage,
  Bookings,
  Orders,
  Maintenance,
  Payments,
  Users,
  Roles,
  Permissions,
  AuditLogs,
  Reports,
  Settings,
  RabbitMQMonitorPage,
  DatabaseMonitorPage,
  WebsocketConnectionsPage,
  EventLogs,
  Login as AdminLogin,
  NotificationsPage,
  HousekeepingPage,
  AccessDenied,
} from './admin/pages';

function App() {
  return (
    <BrowserRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
      <Routes>
        <Route path="/login" element={<AdminLogin />} />
        <Route path="/admin" element={<ProtectedRoute requiredPermissions={['view_dashboard']} />}>
          <Route element={<AdminLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<AdminDashboard />} />
            <Route path="rooms" element={<AdminRoomsPage />} />
            <Route path="bookings" element={<Bookings />} />
            <Route path="orders" element={<Orders />} />
            <Route path="maintenance" element={<Maintenance />} />
            <Route path="housekeeping" element={<HousekeepingPage />} />
            <Route path="reports" element={<Reports />} />
            <Route path="notifications" element={<NotificationsPage />} />
            
            <Route path="payments" element={<Payments />} />
            <Route path="users" element={<Users />} />
            <Route path="roles" element={<Roles />} />
            <Route path="permissions" element={<Permissions />} />
            <Route path="audit-logs" element={<AuditLogs />} />
            <Route path="access-denied" element={<AccessDenied />} />
            <Route path="settings" element={<Settings />} />
            <Route path="settings/infrastructure/rabbitmq" element={<RabbitMQMonitorPage />} />
            <Route path="settings/infrastructure/database" element={<DatabaseMonitorPage />} />
            <Route path="settings/infrastructure/websockets" element={<WebsocketConnectionsPage />} />
            <Route path="settings/infrastructure/event-logs" element={<EventLogs />} />
          </Route>
        </Route>

        <Route element={<GuestLayout />}>
          <Route path="/" element={<Home />} />
          <Route path="/register" element={<Register />} />
          <Route path="/rooms" element={<RoomsPage />} />
          <Route path="/room/:id" element={<RoomDetails />} />
          <Route path="/favorites" element={<Favorites />} />
          <Route path="/checkout" element={<Checkout />} />
          <Route path="/payment" element={<PaymentPage />} />
          <Route path="/profile" element={<Profile />} />
        </Route>

        <Route path="*" element={<PageNotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
