import React, { useEffect, useRef } from 'react';
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
  onPlantClick?: (plant: GardenPlantResponseDTO) => void;
  isSelected?: boolean; // New prop to indicate if this plant is currently selected
}

const GardenCell: React.FC<GardenCellProps> = ({ 
  x, 
  y, 
  plant, 
  isEditMode, 
  onDropPlant, 
  isTempPlant = false,
  onPlantClick,
  isSelected = false // Default to false
}) => {
  const dispatch = useAppDispatch();
  const cellRef = useRef<HTMLDivElement>(null);
  
  // Reset cell styling when a plant is present or edit mode changes
  useEffect(() => {
    if (cellRef.current) {
      // Reset any inline styles that might have been applied during drag operations
      cellRef.current.style.border = '';
      
      // Set the appropriate background color based on whether there's a plant and if it's selected
      if (plant) {
        if (isSelected) {
          cellRef.current.style.backgroundColor = '#3e6d40'; // Brighter green for selected
        } else {
          cellRef.current.style.backgroundColor = '#2d4a2e'; // Green for occupied cells
        }
      } else {
        cellRef.current.style.backgroundColor = '#3a3a3a'; // Default for empty cells
      }
    }
  }, [plant, isEditMode, isSelected]);
  
  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    
    // Check if the cell is occupied
    if (plant) {
      e.currentTarget.style.border = '2px solid #dc3545'; // Red border for occupied cells
      e.currentTarget.style.backgroundColor = '#4a3a3a'; // Darker red background
    } else {
      e.currentTarget.style.border = '2px solid #4caf50'; // Green border for empty cells
      e.currentTarget.style.backgroundColor = '#3a4a3a'; // Darker green background
    }
  };
  
  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.currentTarget.style.border = '';
    if (isSelected && plant) {
      e.currentTarget.style.backgroundColor = '#3e6d40'; // Restore selected state
    } else {
      e.currentTarget.style.backgroundColor = plant ? '#2d4a2e' : '#3a3a3a';
    }
  };
  
  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.currentTarget.style.border = '';
    if (isSelected && plant) {
      e.currentTarget.style.backgroundColor = '#3e6d40'; // Restore selected state
    } else {
      e.currentTarget.style.backgroundColor = plant ? '#2d4a2e' : '#3a3a3a';
    }
    
    // Don't allow dropping on occupied cells
    if (plant) return;
    
    try {
      const data = JSON.parse(e.dataTransfer.getData('application/json'));
      if (data && data.plantId) {
        // Only proceed if this is not the original position
        if (!(data.fromX === x && data.fromY === y)) {
          onDropPlant(x, y, data.plantId, data.name, data.imageBase64);
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
    // Restore proper background color
    if (cellRef.current) {
      if (isSelected && plant) {
        cellRef.current.style.backgroundColor = '#3e6d40'; // Restore selected state
      } else {
        cellRef.current.style.backgroundColor = plant ? '#2d4a2e' : '#3a3a3a';
      }
    }
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

  // Handle click on a plant cell
  const handleCellClick = () => {
    if (!isEditMode && plant && isGardenPlantResponseDTO(plant) && onPlantClick) {
      onPlantClick(plant);
    }
  };

  // Determine if this cell should be clickable
  const isClickable = !isEditMode && plant && isGardenPlantResponseDTO(plant) && onPlantClick;

  return (
    <div 
      ref={cellRef}
      className={`garden-cell ${plant ? 'occupied' : ''} ${isClickable ? 'clickable' : ''} ${isSelected ? 'selected' : ''}`}
      onDragOver={isEditMode ? handleDragOver : undefined}
      onDragLeave={isEditMode ? handleDragLeave : undefined}
      onDrop={isEditMode ? handleDrop : undefined}
      draggable={isEditMode && plant ? true : false}
      onDragStart={isEditMode && plant ? handleDragStart : undefined}
      onDragEnd={handleDragEnd}
      onClick={handleCellClick}
      title={isClickable ? "Click to view plant details" : ""}
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