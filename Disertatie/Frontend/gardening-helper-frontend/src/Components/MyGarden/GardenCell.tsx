import React from 'react';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { removePlantFromGarden } from '../../Redux/Garden/GardenThunk';
import { useAppDispatch } from '../../Redux/Hooks/ReduxHooks';
import DraggablePlantCard from './DraggablePlantCard';
interface GardenCellProps {
  x: number;
  y: number;
  plant?: GardenPlantResponseDTO;
  onDropPlant: (x: number, y: number, plantId: number) => void;
  isEditMode: boolean;
}

const GardenCell: React.FC<GardenCellProps> = ({ x, y, plant, isEditMode, onDropPlant }) => {
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
        onDropPlant(x, y, data.plantId);
      }
    } catch (error) {
      console.error('Error parsing dropped data:', error);
    }
  };

  const handleRemovePlant = () => {
    if (plant) {
      dispatch(removePlantFromGarden(plant.id));
    }
  };

  return (
    <div 
      className={`garden-cell ${plant ? 'occupied' : ''}`}
      onDragOver={isEditMode ? handleDragOver : undefined}
      onDragLeave={isEditMode ? handleDragLeave : undefined}
      onDrop={isEditMode ? handleDrop : undefined}
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
            id={plant.plantId}
            name={plant.plantName}
            imageBase64={plant.base64Image}
            isDraggable={false}
          />
        </div>
      ) : (
        <span className="text-muted"></span>
      )}
    </div>
  );
};

export default GardenCell;