import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { setGardenError, setGardenLoading } from '../../Redux/Garden/GardenSlice';
import { fetchUserGarden } from '../../Redux/Garden/GardenThunk';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectPlantByName } from '../../Redux/Plant/PlantSelector';
import { selectUser } from '../../Redux/User/UserSelector'; // Import the user selector
import { GardenService } from '../../Services/GardenService';
import './ExpandedPlantView.css';

interface ExpandedPlantViewProps {
    plant: GardenPlantResponseDTO;
    onClose: () => void;
    onPlantUpdated?: () => void;  // Optional callback for when plant is updated
  }
  
  const ExpandedPlantView: React.FC<ExpandedPlantViewProps> = ({ plant, onClose, onPlantUpdated }) => {
    const dbPlant = useAppSelector(selectPlantByName(plant.plantName));
    const activeUser = useAppSelector(selectUser); // Get the active user
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [localPlant, setLocalPlant] = useState<GardenPlantResponseDTO>(plant);
  
    // Calculate the days since the plant was last watered
    const daysSinceWatered = () => {
      const lastWatered = new Date(localPlant.lastWateredDate);
      const today = new Date();
      const differenceInTime = today.getTime() - lastWatered.getTime();
      const differenceInDays = Math.floor(differenceInTime / (1000 * 3600 * 24));
      return differenceInDays;
    };

    useEffect(() => {
      setLocalPlant(plant);
    }, [plant]);
  
    // Calculate days left until watering is needed
    const daysUntilWateringNeeded = () => {
      if (!dbPlant || !dbPlant.wateringThresholdDays) return 0;
      
      const daysWatered = daysSinceWatered();
      const daysLeft = dbPlant.wateringThresholdDays - daysWatered;
      return Math.max(0, daysLeft); // Don't return negative days
    };
    
    // Calculate percentage of watering days left
    const wateringDaysPercentage = () => {
      if (!dbPlant || !dbPlant.wateringThresholdDays) return 0;
      
      const daysLeft = daysUntilWateringNeeded();
      return Math.min(100, Math.max(0, (daysLeft / dbPlant.wateringThresholdDays) * 100));
    };
    
    // Get watering status with color
    const getWateringStatus = () => {
      if (!dbPlant || !dbPlant.wateringThresholdDays) {
        return { text: 'Unknown', color: '#888888' };
      }
      
      const daysLeft = daysUntilWateringNeeded();
      const threshold = dbPlant.wateringThresholdDays;
      
      if (daysLeft === 0) {
        return { text: 'Water Now!', color: '#e74c3c' };
      } else if (daysLeft <= Math.ceil(threshold * 0.25)) {
        return { text: 'Soon', color: '#f39c12' };
      } else {
        return { text: 'Good', color: '#2ecc71' };
      }
    };
  
    // Format the date nicely
    const formatDate = (date: Date) => {
      return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    };
    
    // Navigate to plant detail page
    const handleImageClick = () => {
      if (dbPlant && dbPlant.id) {
        navigate(`/plants/${dbPlant.id}`, { state: { from: 'Garden' } });
      }
    };

    // Handle watering the plant
    const handleWaterPlant = async () => {
      if (!localPlant.id) return;
      
      try {
        dispatch(setGardenLoading(true));
        
        // Create a new Date object for the current time
        const currentDate = new Date();
        
        // Update the plant with current date as last watered date
        const updatedGarden = await GardenService.updateGardenPlant({
          gardenPlantId: localPlant.id,
          positionX: localPlant.positionX,
          positionY: localPlant.positionY,
          lastWateredDate: currentDate,
          lastSoilMoisture: localPlant.lastSoilMoisture || 0
        });
        
        // Update local state with the new information
        const updatedPlant = updatedGarden.gardenPlants.find(p => p.id === localPlant.id);
        if (updatedPlant) {
          setLocalPlant(updatedPlant);
        } else {
          // If we can't find the updated plant, at least update the date locally
          setLocalPlant({
            ...localPlant,
            lastWateredDate: currentDate
          });
        }
        
        // Refresh the garden data in Redux store with the user ID
        dispatch(fetchUserGarden(activeUser.id));
        
        // Notify parent component that an update occurred
        if (onPlantUpdated) {
          onPlantUpdated();
        }
        
        dispatch(setGardenLoading(false));
      } catch (error) {
        dispatch(setGardenError(error instanceof Error ? error.message : 'Failed to water plant'));
        dispatch(setGardenLoading(false));
      }
    };

    const wateringStatus = getWateringStatus();
  
    return (
      <div className="expanded-plant-view">
        <div className="expanded-content">
          <div 
            className="expanded-image-container clickable-image"
            onClick={handleImageClick}
            title="Click to view plant details"
          >
            {plant.base64Image ? (
              <img 
                src={localPlant.base64Image} 
                alt={localPlant.plantName} 
                className="expanded-plant-image" 
              />
            ) : (
              <div className="no-image">No image available</div>
            )}
          </div>
          
          <div className="expanded-details">
            <h2 className="expanded-plant-name">{localPlant.plantName}</h2>
            
            <div className="plant-details-grid">
              <div className="detail-item">
                <span className="detail-label">Status:</span>
                <span className="detail-value">{localPlant.status}</span>
              </div>
              
              <div className="detail-item">
                <span className="detail-label">Last Watered:</span>
                <span className="detail-value">
                  {formatDate(localPlant.lastWateredDate)}
                  <span className="days-ago">({daysSinceWatered()} days ago)</span>
                </span>
              </div>
              
              <div className="detail-item">
                <span className="detail-label">Watering Status:</span>
                <div className="watering-wrapper">
                  <div className="watering-bar-container">
                    <div 
                      className="watering-bar" 
                      style={{ 
                        width: `${wateringDaysPercentage()}%`,
                        backgroundColor: wateringStatus.color
                      }}
                    ></div>
                  </div>
                  <span className="watering-value" style={{ color: wateringStatus.color }}>
                    {daysUntilWateringNeeded() === 0 ? (
                      <strong>{wateringStatus.text}</strong>
                    ) : (
                      <>
                        {daysUntilWateringNeeded()} day{daysUntilWateringNeeded() !== 1 ? 's' : ''} left ({wateringStatus.text})
                      </>
                    )}
                  </span>
                </div>
              </div>
              
              <div className="detail-item">
                <span className="detail-label">Position:</span>
                <span className="detail-value">X: {localPlant.positionX}, Y: {localPlant.positionY}</span>
              </div>
            </div>
            
            <div className="plant-actions">
              <button 
                className="btn btn-outline-primary" 
                onClick={handleWaterPlant}
                disabled={daysSinceWatered() === 0}
              >
                {daysSinceWatered() === 0 ? 'Watered Today' : 'Water Plant'}
              </button>
            </div>
          </div>
        </div>
        
        {/* Improved close button with better visual feedback */}
        <button 
          className="close-button" 
          onClick={onClose} 
          aria-label="Close plant details"
          title="Close plant details"
        >
          <span className="close-button-icon">×</span>
        </button>
      </div>
    );
  };
  
  export default ExpandedPlantView;