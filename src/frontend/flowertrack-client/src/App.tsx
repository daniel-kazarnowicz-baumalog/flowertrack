import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ServiceLoginPage } from './pages/service';
import { ClientLoginPage } from './pages/client';
import { ServiceLanding, ClientLanding } from './pages/landing';
import NotFound from './pages/NotFound';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Landing Pages */}
        <Route path="/" element={<ServiceLanding />} />
        <Route path="/service" element={<ServiceLanding />} />
        <Route path="/client" element={<ClientLanding />} />

        {/* Login Pages */}
        <Route path="/service/login" element={<ServiceLoginPage />} />
        <Route path="/client/login" element={<ClientLoginPage />} />

        {/* 404 Not Found */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
