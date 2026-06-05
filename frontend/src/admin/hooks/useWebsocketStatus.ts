import { useEffect, useState } from 'react';
import { createHotelHubConnection, hasValidAccessToken } from '../lib/signalr';

export function useWebsocketStatus() {
  const [connected, setConnected] = useState(false);
  const [reconnecting, setReconnecting] = useState(false);

  useEffect(() => {
    if (!hasValidAccessToken()) {
      setConnected(false);
      setReconnecting(false);
      return;
    }

    const connection = createHotelHubConnection();

    connection.onreconnecting(() => {
      setConnected(false);
      setReconnecting(true);
    });

    connection.onreconnected(() => {
      setConnected(true);
      setReconnecting(false);
    });

    connection.onclose(() => {
      setConnected(false);
      setReconnecting(false);
    });

    connection.start()
      .then(() => setConnected(true))
      .catch(() => setConnected(false));

    return () => { connection.stop(); };
  }, []);

  return { connected, reconnecting };
}
