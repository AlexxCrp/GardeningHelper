import React, { useEffect, useMemo, useState } from 'react';
import { CreateGardenRequestDTO } from '../../Models/API/DTOs/Auto/Request/createGardenRequestDTO';
import { selectGarden, selectGardenError, selectGardenLoading, selectIsCreatingGarden } from '../../Redux/Garden/GardenSelector';
import { toggleCreateGardenForm } from '../../Redux/Garden/GardenSlice';
import { addPlantToGarden, fetchUserGarden, updateGarden } from '../../Redux/Garden/GardenThunk';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectAllPlants } from '../../Redux/Plant/PlantSelector';
import { selectUser } from '../../Redux/User/UserSelector';
import AvailablePlants from './AvailablePlants';
import CreateGardenForm from './CreateGardenForm';
import './Garden.css';
import GardenGrid from './GardenGrid';

const Garden: React.FC = () => {
  const dispatch = useAppDispatch();
  const garden = useAppSelector(selectGarden);
  const activeUser = useAppSelector(selectUser);
  const isLoading = useAppSelector(selectGardenLoading);
  const error = useAppSelector(selectGardenError);
  const isCreatingGarden = useAppSelector(selectIsCreatingGarden);
  const allPlants = useAppSelector(selectAllPlants);

  const [isEditMode, setIsEditMode] = useState(false);
  const [tempXSize, setTempXSize] = useState(garden?.xSize || 5);
  const [tempYSize, setTempYSize] = useState(garden?.ySize || 5);

  useEffect(() => {
    dispatch(fetchUserGarden(activeUser.id));
  }, [dispatch, activeUser.id]);

  useEffect(() => {
    if (garden) {
      setTempXSize(garden.xSize);
      setTempYSize(garden.ySize);
    }
  }, [garden]);

  // Calculate plants that would be removed with current size settings
  const plantsAtRisk = useMemo(() => {
    if (!garden) return 0;
    
    return garden.gardenPlants.filter(
      plant => plant.positionX >= tempXSize || plant.positionY >= tempYSize
    ).length;
  }, [garden, tempXSize, tempYSize]);

  const handleShowCreateForm = () => {
    dispatch(toggleCreateGardenForm());
  };

  const handlePlantDropped = (x: number, y: number, plantId: number) => {
    if (!garden) return;
    const plantAlreadyExists = garden.gardenPlants.some(p => p.positionX === x && p.positionY === y);
    if (plantAlreadyExists) {
        console.warn(`Position (<span class="math-inline">\{x\},</span>{y}) is already occupied.`);
        return;
    };
  
    dispatch(addPlantToGarden({ plantId, positionX: x, positionY: y }));
  };

  const handleSaveChanges = async () => {
    if (!garden) return;
    
    // If plants will be removed, show confirmation dialog
    if (plantsAtRisk > 0) {
      const confirmMessage = `Warning: ${plantsAtRisk} plant${plantsAtRisk === 1 ? '' : 's'} will be removed by reducing the garden size. Continue?`;
      if (!window.confirm(confirmMessage)) {
        return;
      }
    }
    
    if (tempXSize !== garden.xSize || tempYSize !== garden.ySize) {      
      const newGarden: CreateGardenRequestDTO = { xSize: tempXSize, ySize: tempYSize };
      await dispatch(updateGarden(newGarden));
      
      // Fetch updated garden to ensure plants are displayed correctly
      await dispatch(fetchUserGarden(activeUser.id));
    }
    
    setIsEditMode(false);
  };

  const handleCancelEdit = () => {
    if (garden) {
      setTempXSize(garden.xSize);
      setTempYSize(garden.ySize);
    }
    setIsEditMode(false);
  };

  if (isLoading && !garden) {
    return (
      <div className="garden-container">
        <div className="garden-header">
          <h2 className="garden-title">My Garden</h2>
        </div>
        <div className="text-center p-5">
          <div className="spinner-border text-success" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
          <p className="mt-3">Loading your garden...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="garden-container">
        <div className="garden-header">
          <h2 className="garden-title">My Garden</h2>
        </div>
        <div className="alert alert-danger">An error occurred: {error}</div>
      </div>
    );
  }

  if (!garden) {
    return (
      <div className="garden-container">
        <div className="garden-header">
          <h2 className="garden-title">My Garden</h2>
        </div>
        {isCreatingGarden ? (
          <CreateGardenForm onCancel={handleShowCreateForm} />
        ) : (
          <div className="garden-empty">
            <h3>Welcome to Your Garden Space</h3>
            <p>You don't have a garden yet. Let's create one to start planting!</p>
            <button className="btn auth-submit-btn" onClick={handleShowCreateForm}>
              Create My Garden
            </button>
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="garden-container">
      <div className="garden-header">
        <h2 className="garden-title">My Garden</h2>
        <div className="garden-header-details">
          <span className="badge bg-secondary me-2">
            Size: {isEditMode ? `${tempXSize} × ${tempYSize}` : `${garden.xSize} × ${garden.ySize}`} meters
          </span>
          <span className="badge bg-success">
            {garden.gardenPlants.length} plants
          </span>
        </div>
      </div>

      <GardenGrid 
        garden={garden}
        onPlantDropped={isEditMode ? handlePlantDropped : () => {}}
        isEditMode={isEditMode}
        tempXSize={tempXSize}
        tempYSize={tempYSize}
        setTempXSize={isEditMode ? setTempXSize : undefined}
        setTempYSize={isEditMode ? setTempYSize : undefined}
        plantsAtRisk={plantsAtRisk}
      />

      <div className="garden-actions">
        {!isEditMode ? (
          <button className="btn btn-success" onClick={() => setIsEditMode(true)}>
            Edit Garden
          </button>
        ) : (
          <div className="action-buttons">
            <button 
              className={`btn ${plantsAtRisk > 0 ? 'btn-warning' : 'btn-primary'}`} 
              onClick={handleSaveChanges}
            >
              {plantsAtRisk > 0 ? 'Save (Remove Plants)' : 'Save Changes'}
            </button>
            <button className="btn btn-danger" onClick={handleCancelEdit}>
              Cancel
            </button>
          </div>
        )}
      </div>

      {isEditMode && (
        <div className="available-plants-container">
          <AvailablePlants />
        </div>
      )}
    </div>
  );
};

export default Garden;