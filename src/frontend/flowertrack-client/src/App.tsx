import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Gateway from './pages/Gateway';
import ServiceLogin from './pages/ServiceLogin';
import ClientLogin from './pages/ClientLogin';
import NotFound from './pages/NotFound';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Gateway />} />
        <Route path="/service" element={<ServiceLogin />} />
        <Route path="/client" element={<ClientLogin />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
