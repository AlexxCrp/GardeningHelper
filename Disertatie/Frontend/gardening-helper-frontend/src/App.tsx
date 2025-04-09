import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import './App.css';
import NavBar from './Components/NavBar/NavBar';
import PlantCardContainer from './Components/PlantCardContainer/PlantCardContainer';
import PlantsList from './Components/plantList';

function App() {
  return (
    <Router>
      <div className="app-container">
        <NavBar />
        <div className="page-content container mt-4">
          <Routes>
            <Route path="/" element={<PlantsList/>} />
            <Route path="/plants" element={<PlantCardContainer />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
