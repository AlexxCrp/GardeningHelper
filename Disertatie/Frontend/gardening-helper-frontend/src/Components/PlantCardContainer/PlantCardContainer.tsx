import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectPlantCards, selectPlantsError, selectPlantsLoading } from '../../Redux/Plant/PlantSelector';
import { fetchCardPlants } from '../../Redux/Plant/PlantThunk';
import PlantCard from '../PlantCard/PlantCard';
import './PlantCardContainer.css';

const PlantCardContainer: React.FC = () => {
  const dispatch = useAppDispatch();
  const plants = useAppSelector(selectPlantCards);
  const isLoading = useAppSelector(selectPlantsLoading);
  const error = useAppSelector(selectPlantsError);

  useEffect(() => {
    dispatch(fetchCardPlants());
  }, [dispatch]);

  if (isLoading) return <p className="loading-text">Loading...</p>;
  if (error) return <p className="error-text">{error}</p>;

  return (
    <div className="plant-container">
      <div className="plant-grid">
        {plants.map((plant, index) => (
          <PlantCard 
            key={index} 
            name={plant.name} 
            imageBase64={plant.imageBase64} 
          />
        ))}
      </div>
    </div>
  );
};

export default PlantCardContainer;