import { BrowserRouter, Routes, Route } from 'react-router-dom';
import ServiceLanding from './pages/ServiceLanding';
import ClientLanding from './pages/ClientLanding';
import NotFound from './pages/NotFound';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ServiceLanding />} />
        <Route path="/service" element={<ServiceLanding />} />
        <Route path="/client" element={<ClientLanding />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
