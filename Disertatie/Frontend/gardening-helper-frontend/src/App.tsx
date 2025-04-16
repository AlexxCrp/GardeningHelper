// src/App.tsx
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import './App.css';
import ProtectedRoute from './Components/Auth/ProtectedRoute';
import Login from './Components/Login/Login';
import NavBar from './Components/NavBar/NavBar';
import PlantCardContainer from './Components/PlantCardContainer/PlantCardContainer';
import PlantsList from './Components/plantList';
import Register from './Components/Register/Register';

function App() {
  return (
    <Router>
      <div className="app-container">
        <NavBar />
        <div className="page-content container mt-4">
          <Routes>
            {/* Public routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            
            {/* Protected routes */}
            <Route 
              path="/" 
              element={
                <ProtectedRoute>
                  <PlantsList />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/plants" 
              element={
                <ProtectedRoute>
                  <PlantCardContainer />
                </ProtectedRoute>
              } 
            />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;