import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectAllPlants, selectPlantsError, selectPlantsLoading } from '../../Redux/Plant/PlantSelector';
import { fetchAllPlants } from '../../Redux/Plant/PlantThunk';
import PlantCard from '../PlantCard/PlantCard';
import './PlantCardContainer.css';

const PlantCardContainer: React.FC = () => {
  const dispatch = useAppDispatch();
  const plants = useAppSelector(selectAllPlants);
  const isLoading = useAppSelector(selectPlantsLoading);
  const error = useAppSelector(selectPlantsError);

  useEffect(() => {
    dispatch(fetchAllPlants());
  }, [dispatch]);

  if (isLoading) return <p className="loading-text">Loading plants...</p>;
  if (error) return <p className="error-text">{error}</p>;

  return (
    <div className="plant-container">
      <h1 className="text-green">Plant Collection</h1>
      <p>Click on a plant to view detailed information</p>
      <div className="plant-grid">
        {plants.map((plant) => (
          <PlantCard 
            key={plant.id} 
            id={plant.id}
            name={plant.name} 
            imageBase64={plant.imageBase64} 
          />
        ))}
      </div>
      {plants.length === 0 && <p className="no-plants-message">No plants found in your collection.</p>}
    </div>
  );
};

export default PlantCardContainer;