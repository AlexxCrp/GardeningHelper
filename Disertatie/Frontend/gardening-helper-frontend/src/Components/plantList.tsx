import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../Redux/Hooks/ReduxHooks';
import { selectPlantCards, selectPlantsError, selectPlantsLoading } from '../Redux/Plant/PlantSelector';
import { fetchCardPlants } from '../Redux/Plant/PlantThunk';

const PlantsList: React.FC = () => {
  const dispatch = useAppDispatch();
  const plants = useAppSelector(selectPlantCards);
  const isLoading = useAppSelector(selectPlantsLoading);
  const error = useAppSelector(selectPlantsError);

  // Optional: fetch plants on component mount
  useEffect(() => {
    // You can uncomment this if you want to load plants when the component mounts
    // dispatch(fetchAllPlants());
  }, [dispatch]);

  const handleFetchPlants = () => {
    dispatch(fetchCardPlants());
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Plants List</h1>
      <button 
        onClick={handleFetchPlants} 
        disabled={isLoading}
        style={{
          padding: '10px 15px',
          backgroundColor: '#4CAF50',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer',
          marginBottom: '20px'
        }}
      >
        {isLoading ? 'Loading...' : 'Get All Plants'}
      </button>

      {error && (
        <div style={{ color: 'red', marginBottom: '20px' }}>
          Error: {error}
        </div>
      )}

      {plants.length > 0 ? (
        <div>
          <h2>Plants Data</h2>
          <pre style={{
            backgroundColor: '#f5f5f5',
            padding: '15px',
            borderRadius: '4px',
            overflowX: 'auto'
          }}>
            {JSON.stringify(plants, null, 2)}
          </pre>
        </div>
      ) : (
        !isLoading && <p>No plants found.</p>
      )}
    </div>
  );
};

export default PlantsList;