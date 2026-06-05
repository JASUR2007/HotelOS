import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import ErrorBoundary from './components/ErrorBoundary';
import { RoomContext } from './context/RoomContext';
import './style/fonts.css';
import './style/index.css';

const rootEl = document.getElementById('root');
if (!rootEl) throw new Error('Root element not found');

createRoot(rootEl).render(
  <ErrorBoundary>
    <RoomContext>
      <StrictMode>
        <App />
      </StrictMode>
    </RoomContext>
  </ErrorBoundary>
);
