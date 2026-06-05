import { useEffect } from 'react';
import { createHotelHubConnection } from '../lib/signalr';
import { useDashboardStore } from '../store/dashboardStore';

export function useRealtimeNotifications() {
  const prependNotification = useDashboardStore((s) => s.prependNotification);

  useEffect(() => {
    const connection = createHotelHubConnection();

    connection.on('NotificationReceived', (notification: any) => {
      prependNotification({
        id: Date.now() + Math.random(),
        title: notification.title,
        message: notification.message,
        severity: notification.type === 'maintenance' || notification.type === 'critical'
          ? 'critical' : notification.type === 'warning' ? 'warning' : 'info',
        createdAt: notification.createdAt ?? new Date().toISOString(),
      });
    });

    connection.on('BookingCreated', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Booking Created', message: `Room ${data?.roomId ?? 'assigned'} booked for ${data?.guestName ?? 'guest'}.`, severity: 'info', createdAt: new Date().toISOString() });
    });

    connection.on('BookingCancelled', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Booking Cancelled', message: `Booking #${data?.bookingId ?? 'N/A'} has been cancelled.`, severity: 'warning', createdAt: new Date().toISOString() });
    });

    connection.on('GuestCheckedIn', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Guest Checked In', message: `${data?.guestName ?? 'Guest'} checked into Room #${data?.roomId ?? 'N/A'}.`, severity: 'info', createdAt: new Date().toISOString() });
    });

    connection.on('GuestCheckedOut', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Guest Checked Out', message: `Room #${data?.roomId ?? 'N/A'} vacated.`, severity: 'info', createdAt: new Date().toISOString() });
    });

    connection.on('OrderDelivered', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Order Delivered', message: `Order #${data?.orderId ?? 'N/A'} delivered to Room ${data?.roomNumber ?? 'N/A'}.`, severity: 'info', createdAt: new Date().toISOString() });
    });

    connection.on('MaintenanceCreated', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Maintenance Created', message: data?.title ?? 'New maintenance issue reported.', severity: 'warning', createdAt: new Date().toISOString() });
    });

    connection.on('MaintenanceCompleted', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Maintenance Resolved', message: `Ticket #${data?.ticketId ?? 'N/A'} resolved.`, severity: 'info', createdAt: new Date().toISOString() });
    });

    connection.on('PaymentCompleted', (data: any) => {
      prependNotification({ id: Date.now() + Math.random(), title: 'Payment Received', message: `${data?.amount ? '$' + data.amount : 'Payment'} received for Invoice #${data?.invoiceId ?? 'N/A'}.`, severity: 'info', createdAt: new Date().toISOString() });
    });

    const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
      | { getState(): { accessToken?: string } }
      | undefined;
    const token = store?.getState().accessToken ?? '';
    if (!token || !token.startsWith('eyJ')) {
      console.info('[SignalR] Skipping connection — no valid access token');
      return;
    }

    connection.start().catch(() => undefined);

    return () => {
      connection.stop().catch(() => undefined);
    };
  }, [prependNotification]);
}
