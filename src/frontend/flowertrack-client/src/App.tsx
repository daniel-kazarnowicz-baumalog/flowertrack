import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ServiceLoginPage } from './pages/service';
import { ClientLoginPage } from './pages/client';
import NotFound from './pages/NotFound';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Root redirects to service login */}
        <Route path="/" element={<Navigate to="/service/login" replace />} />

        {/* Service Portal Routes */}
        <Route path="/service" element={<Navigate to="/service/login" replace />} />
        <Route path="/service/login" element={<ServiceLoginPage />} />

        {/* Client Portal Routes */}
        <Route path="/client" element={<Navigate to="/client/login" replace />} />
        <Route path="/client/login" element={<ClientLoginPage />} />

        {/* 404 Not Found */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
