import React from 'react';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { removeTempPlant } from '../../Redux/Garden/GardenSlice';
import { TempGardenPlant } from '../../Redux/Garden/GardenState';
import { useAppDispatch } from '../../Redux/Hooks/ReduxHooks';
import DraggablePlantCard from './DraggablePlantCard';

// Add explicit type guard functions to check plant types
function isGardenPlantResponseDTO(plant: any): plant is GardenPlantResponseDTO {
  return plant && 'plantId' in plant && 'plantName' in plant;
}

function isTempGardenPlant(plant: any): plant is TempGardenPlant {
  return plant && 'id' in plant && 'name' in plant;
}

interface GardenCellProps {
  x: number;
  y: number;
  plant?: GardenPlantResponseDTO | TempGardenPlant;
  onDropPlant: (x: number, y: number, plantId: number, plantName: string, imageBase64?: string | null) => void;
  isEditMode: boolean;
  isTempPlant?: boolean;
}

const GardenCell: React.FC<GardenCellProps> = ({ x, y, plant, isEditMode, onDropPlant, isTempPlant = false }) => {
  const dispatch = useAppDispatch();
  
  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.currentTarget.style.backgroundColor = '#4a6b4b';
  };

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.currentTarget.style.backgroundColor = '';
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.currentTarget.style.backgroundColor = '';
    
    try {
      const data = JSON.parse(e.dataTransfer.getData('application/json'));
      if (data && data.plantId) {
        // Pass all plant data to the drop handler
        onDropPlant(x, y, data.plantId, data.name, data.imageBase64);
        
        // If this is a move operation, remove the plant from the original cell
        if (data.isMoveOperation && data.fromX !== undefined && data.fromY !== undefined) {
          dispatch(removeTempPlant({ x: data.fromX, y: data.fromY }));
        }
      }
    } catch (error) {
      console.error('Error parsing dropped data:', error);
    }
  };

  const handleRemovePlant = () => {
    if (plant) {
      if (isTempPlant) {
        // Only remove from temp garden during edit mode
        dispatch(removeTempPlant({ x, y }));
      }
    }
  };

  const handleDragStart = (e: React.DragEvent<HTMLDivElement>) => {
    if (!isEditMode || !plant) return;
    
    // Use type guards to safely access properties
    const plantId = isGardenPlantResponseDTO(plant) ? plant.plantId : 
                    isTempGardenPlant(plant) ? plant.id : 0;
                    
    const plantName = isGardenPlantResponseDTO(plant) ? plant.plantName : 
                      isTempGardenPlant(plant) ? plant.plantName : '';
                      
    const imageBase64 = plant.base64Image;
    
    // Set data for moving existing plants
    const dragData = {
      plantId,
      name: plantName,
      imageBase64,
      fromX: x,
      fromY: y,
      isMoveOperation: true // Flag to identify this as a move operation
    };
    
    e.dataTransfer.setData('application/json', JSON.stringify(dragData));
    // Also store in localStorage as a backup
    localStorage.setItem('dragData', JSON.stringify(dragData));
    
    e.currentTarget.classList.add('dragging');
  };

  const handleDragEnd = (e: React.DragEvent<HTMLDivElement>) => {
    e.currentTarget.classList.remove('dragging');
  };

  // Helper function to safely get plant ID
  const getPlantId = (): number => {
    if (!plant) return 0;
    return isGardenPlantResponseDTO(plant) ? plant.plantId : 
           isTempGardenPlant(plant) ? plant.id! : 0;
  };

  // Helper function to safely get plant name
  const getPlantName = (): string => {
    if (!plant) return '';
    return isGardenPlantResponseDTO(plant) ? plant.plantName : 
           isTempGardenPlant(plant) ? plant.plantName : '';
  };

  // Helper function to safely get plant image
  const getPlantImage = (): string | null => {
    if (!plant) return null;
    return plant.base64Image || null;
  };

  return (
    <div 
      className={`garden-cell ${plant ? 'occupied' : ''}`}
      onDragOver={isEditMode ? handleDragOver : undefined}
      onDragLeave={isEditMode ? handleDragLeave : undefined}
      onDrop={isEditMode ? handleDrop : undefined}
      draggable={isEditMode && plant ? true : false}
      onDragStart={isEditMode && plant ? handleDragStart : undefined}
      onDragEnd={handleDragEnd}
    >
      {plant ? (
        <div className="garden-cell-content">
          {isEditMode && (
            <button 
              className="cell-remove-btn" 
              onClick={handleRemovePlant}
              title="Remove plant"
            >
              ×
            </button>
          )}
          <DraggablePlantCard
            id={getPlantId()}
            name={getPlantName()}
            imageBase64={getPlantImage()}
            isDraggable={false}
            inGardenCell={true}
          />
        </div>
      ) : (
        <span className="text-muted"></span>
      )}
    </div>
  );
};

export default GardenCell;