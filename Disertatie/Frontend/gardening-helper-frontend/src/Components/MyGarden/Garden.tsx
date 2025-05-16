import React, { useEffect, useMemo, useState } from 'react';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import {
  selectGarden,
  selectGardenError,
  selectGardenLoading,
  selectIsCreatingGarden,
  selectTempGarden
} from '../../Redux/Garden/GardenSelector';
import {
  addTempPlant,
  cancelEditMode,
  initializeEditMode,
  toggleCreateGardenForm,
  updateTempGardenSize
} from '../../Redux/Garden/GardenSlice';
import {
  batchUpdateGarden,
  fetchUserGarden
} from '../../Redux/Garden/GardenThunk';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectAllPlants } from '../../Redux/Plant/PlantSelector';
import { fetchAllPlants } from '../../Redux/Plant/PlantThunk';
import { selectUser } from '../../Redux/User/UserSelector';
import AvailablePlants from './AvailablePlants';
import CreateGardenForm from './CreateGardenForm';
import ExpandedPlantView from './ExpandedPlantView';
import './Garden.css';
import GardenGrid from './GardenGrid';

const Garden: React.FC = () => {
  const dispatch = useAppDispatch();
  const garden = useAppSelector(selectGarden);
  const tempGarden = useAppSelector(selectTempGarden);
  const activeUser = useAppSelector(selectUser);
  const isLoading = useAppSelector(selectGardenLoading);
  const error = useAppSelector(selectGardenError);
  const isCreatingGarden = useAppSelector(selectIsCreatingGarden);
  const allPlants = useAppSelector(selectAllPlants);

  const [isEditMode, setIsEditMode] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0); // Add a key to force re-render
  const [selectedPlant, setSelectedPlant] = useState<GardenPlantResponseDTO | null>(null);

  useEffect(() => {
    dispatch(fetchUserGarden(activeUser.id));
    dispatch(fetchAllPlants());
  }, [dispatch, activeUser.id]);

  // Calculate plants that would be removed with current size settings
  const plantsAtRisk = useMemo(() => {
    if (!garden || !tempGarden) return 0;
    
    return tempGarden.plants.filter(
      plant => 
        plant.operation !== 'remove' && 
        (plant.positionX >= tempGarden.xSize || plant.positionY >= tempGarden.ySize)
    ).length;
  }, [garden, tempGarden]);

  const handleShowCreateForm = () => {
    dispatch(toggleCreateGardenForm());
  };

  const handleEnterEditMode = () => {
    dispatch(initializeEditMode());
    setIsEditMode(true);
    // Close the expanded view if open when entering edit mode
    setSelectedPlant(null);
  };

  const handlePlantDropped = (x: number, y: number, plantId: number) => {
    if (!garden || !tempGarden) return;
    
    // Find the plant in available plants to get its name and image
    const plant = allPlants.find(p => p.id === plantId);
    if (!plant) return;

    dispatch(addTempPlant({
      plantId: plantId,
      plantName: plant.name,
      positionX: x,
      positionY: y,
      base64Image: plant.imageBase64
    }));
  };

  const handleSaveChanges = async () => {
    if (!garden || !tempGarden) return;
    
    // If plants will be removed, show confirmation dialog
    if (plantsAtRisk > 0) {
      const confirmMessage = `Warning: ${plantsAtRisk} plant${plantsAtRisk === 1 ? '' : 's'} will be removed by reducing the garden size. Continue?`;
      if (!window.confirm(confirmMessage)) {
        return;
      }
    }
    
    try {
      await dispatch(batchUpdateGarden()).unwrap();
      
      // Fetch updated garden to ensure plants are displayed correctly
      await dispatch(fetchUserGarden(activeUser.id));
      
      setIsEditMode(false);
      // Force re-render to reset all cell styling
      setRefreshKey(prev => prev + 1);
    } catch (error) {
      console.error("Failed to save changes:", error);
    }
  };

  const handleCancelEdit = () => {
    dispatch(cancelEditMode());
    setIsEditMode(false);
    // Force re-render to reset all cell styling
    setRefreshKey(prev => prev + 1);
  };

  const handleSizeChange = (xSize: number, ySize: number) => {
    dispatch(updateTempGardenSize({ xSize, ySize }));
  };

  // Handle plant selection for expanded view
  const handlePlantClick = (plant: GardenPlantResponseDTO) => {
    setSelectedPlant(plant);
  };

  // Handle closing the expanded view
  const handleCloseExpandedView = () => {
    setSelectedPlant(null);
  };

  // Handle plant updated event (from ExpandedPlantView)
  const handlePlantUpdated = () => {
    // Refresh the garden data
    dispatch(fetchUserGarden(activeUser.id));
    // Update the selected plant with the latest data
    if (selectedPlant && garden) {
      const updatedPlant = garden.gardenPlants.find(p => p.id === selectedPlant.id);
      if (updatedPlant) {
        setSelectedPlant(updatedPlant);
      }
    }
    // Increment refresh key to force re-render
    setRefreshKey(prev => prev + 1);
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
    <div className={`garden-container ${isEditMode ? 'edit-mode' : ''}`}>
      <div className="garden-header">
        <h2 className="garden-title">My Garden</h2>
        <div className="garden-header-details">
          <span className="badge bg-secondary me-2">
            Size: {isEditMode && tempGarden ? `${tempGarden.xSize} × ${tempGarden.ySize}` : `${garden.xSize} × ${garden.ySize}`} meters
          </span>
          <span className="badge bg-success">
            {isEditMode && tempGarden 
              ? tempGarden.plants.filter(p => p.operation !== 'remove').length 
              : garden.gardenPlants.length} plants
          </span>
        </div>
      </div>

      <GardenGrid 
        key={refreshKey} // Add key to force re-mount
        garden={garden}
        tempGarden={tempGarden}
        onPlantDropped={isEditMode ? handlePlantDropped : () => {}}
        isEditMode={isEditMode}
        plantsAtRisk={plantsAtRisk}
        onSizeChange={handleSizeChange}
        onPlantClick={handlePlantClick} // Pass the handler for plant click
        selectedPlant={selectedPlant} // Pass the selected plant to highlight it
      />

      {/* Show ExpandedPlantView when a plant is selected */}
      {selectedPlant && !isEditMode && (
        <ExpandedPlantView
          plant={selectedPlant}
          onClose={handleCloseExpandedView}
          onPlantUpdated={handlePlantUpdated} // Pass the handler for plant updated
        />
      )}

      <div className="garden-actions">
        {!isEditMode ? (
          <button className="btn btn-success" onClick={handleEnterEditMode}>
            Edit Garden
          </button>
        ) : (
          <div className="action-buttons">
            <button 
              className={`btn ${plantsAtRisk > 0 ? 'btn-warning' : 'btn-primary'}`} 
              onClick={handleSaveChanges}
              disabled={isLoading}
            >
              {isLoading ? 'Saving...' : plantsAtRisk > 0 ? 'Save (Remove Plants)' : 'Save Changes'}
            </button>
            <button 
              className="btn btn-danger" 
              onClick={handleCancelEdit}
              disabled={isLoading}
            >
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