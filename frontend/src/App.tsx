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
            <Route path="dashboard" element={<ProtectedRoute requiredPermissions={['view_dashboard']}><AdminDashboard /></ProtectedRoute>} />
            <Route path="rooms" element={<ProtectedRoute requiredPermissions={['manage_rooms']}><AdminRoomsPage /></ProtectedRoute>} />
            <Route path="bookings" element={<ProtectedRoute requiredPermissions={['create_booking']}><Bookings /></ProtectedRoute>} />
            <Route path="orders" element={<ProtectedRoute requiredPermissions={['create_orders']}><Orders /></ProtectedRoute>} />
            <Route path="maintenance" element={<ProtectedRoute requiredPermissions={['view_maintenances']}><Maintenance /></ProtectedRoute>} />
            <Route path="housekeeping" element={<ProtectedRoute requiredPermissions={['manage_rooms']}><HousekeepingPage /></ProtectedRoute>} />
            <Route path="reports" element={<ProtectedRoute requiredPermissions={['view_reports']}><Reports /></ProtectedRoute>} />
            <Route path="notifications" element={<ProtectedRoute allowedRoles={[]}><NotificationsPage /></ProtectedRoute>} />
            <Route path="payments" element={<ProtectedRoute requiredPermissions={['manage_payments']}><Payments /></ProtectedRoute>} />
            <Route path="users" element={<ProtectedRoute requiredPermissions={['manage_users']}><Users /></ProtectedRoute>} />
            <Route path="roles" element={<ProtectedRoute requiredPermissions={['manage_roles']}><Roles /></ProtectedRoute>} />
            <Route path="permissions" element={<ProtectedRoute requiredPermissions={['manage_permissions']}><Permissions /></ProtectedRoute>} />
            <Route path="audit-logs" element={<ProtectedRoute requiredPermissions={['view_audit_logs']}><AuditLogs /></ProtectedRoute>} />
            <Route path="access-denied" element={<AccessDenied />} />
            <Route path="settings" element={<ProtectedRoute requiredPermissions={['view_settings']}><Settings /></ProtectedRoute>} />
            <Route path="settings/infrastructure/rabbitmq" element={<ProtectedRoute requiredPermissions={['view_settings']}><RabbitMQMonitorPage /></ProtectedRoute>} />
            <Route path="settings/infrastructure/database" element={<ProtectedRoute requiredPermissions={['view_settings']}><DatabaseMonitorPage /></ProtectedRoute>} />
            <Route path="settings/infrastructure/websockets" element={<ProtectedRoute requiredPermissions={['view_settings']}><WebsocketConnectionsPage /></ProtectedRoute>} />
            <Route path="settings/infrastructure/event-logs" element={<ProtectedRoute requiredPermissions={['view_settings']}><EventLogs /></ProtectedRoute>} />
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
