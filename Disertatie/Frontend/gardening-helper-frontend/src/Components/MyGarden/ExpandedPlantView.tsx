import React from 'react';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectPlantByName } from '../../Redux/Plant/PlantSelector';
import './ExpandedPlantView.css';

interface ExpandedPlantViewProps {
    plant: GardenPlantResponseDTO;
    onClose: () => void;
  }
  
  const ExpandedPlantView: React.FC<ExpandedPlantViewProps> = ({ plant, onClose }) => {
    const dbPlant = useAppSelector(selectPlantByName(plant.plantName));
  
    // Calculate the days since the plant was last watered
    const daysSinceWatered = () => {
      const lastWatered = new Date(plant.lastWateredDate);
      const today = new Date();
      const differenceInTime = today.getTime() - lastWatered.getTime();
      const differenceInDays = Math.floor(differenceInTime / (1000 * 3600 * 24));
      return differenceInDays;
    };
  
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

    const wateringStatus = getWateringStatus();
  
    return (
      <div className="expanded-plant-view">
        <div className="expanded-content">
          <div className="expanded-image-container">
            {plant.base64Image ? (
              <img 
                src={plant.base64Image} 
                alt={plant.plantName} 
                className="expanded-plant-image" 
              />
            ) : (
              <div className="no-image">No image available</div>
            )}
          </div>
          
          <div className="expanded-details">
            <h2 className="expanded-plant-name">{plant.plantName}</h2>
            
            <div className="plant-details-grid">
              <div className="detail-item">
                <span className="detail-label">Status:</span>
                <span className="detail-value">{plant.status}</span>
              </div>
              
              <div className="detail-item">
                <span className="detail-label">Last Watered:</span>
                <span className="detail-value">
                  {formatDate(plant.lastWateredDate)}
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
                <span className="detail-value">X: {plant.positionX}, Y: {plant.positionY}</span>
              </div>
            </div>
            
            <div className="plant-actions">
              <button className="btn btn-outline-primary">Water Plant</button>
              <button className="btn btn-outline-success">Update Status</button>
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