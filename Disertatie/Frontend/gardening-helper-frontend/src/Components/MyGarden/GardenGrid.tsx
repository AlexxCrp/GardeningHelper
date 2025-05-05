import React, { JSX } from 'react';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { UserGardenResponseDTO } from '../../Models/API/DTOs/Auto/Response/userGardenResponseDTO';
import { moveTempPlant } from '../../Redux/Garden/GardenSlice';
import { TempGardenPlant } from '../../Redux/Garden/GardenState';
import { useAppDispatch } from '../../Redux/Hooks/ReduxHooks';
import GardenCell from './GardenCell';

interface GardenGridProps {
  garden: UserGardenResponseDTO;
  tempGarden: {
    plants: TempGardenPlant[];
    xSize: number;
    ySize: number;
  } | null;
  onPlantDropped: (x: number, y: number, plantId: number) => void;
  isEditMode: boolean;
  plantsAtRisk: number;
  onSizeChange: (x: number, y: number) => void;
  onPlantClick?: (plant: GardenPlantResponseDTO) => void;
  selectedPlant?: GardenPlantResponseDTO | null; // New prop to track the selected plant
}

const GardenGrid: React.FC<GardenGridProps> = ({ 
  garden, 
  tempGarden,
  isEditMode, 
  onPlantDropped,
  plantsAtRisk,
  onSizeChange,
  onPlantClick,
  selectedPlant
}) => {
  const dispatch = useAppDispatch();

  // Find plant in temp garden during edit mode, or from main garden when not in edit mode
  const findPlantAtPosition = (x: number, y: number): GardenPlantResponseDTO | TempGardenPlant | undefined => {
    if (isEditMode && tempGarden) {
      // In edit mode, use temp garden plants that are not marked for removal
      return tempGarden.plants.find(
        p => p.positionX === x && p.positionY === y && p.operation !== 'remove'
      );
    } else {
      // In normal mode, use regular garden plants
      return garden.gardenPlants.find(
        plant => plant.positionX === x && plant.positionY === y
      );
    }
  };

  // Check if a plant at given position is selected
  const isPlantSelected = (x: number, y: number): boolean => {
    if (!selectedPlant) return false;
    return selectedPlant.positionX === x && selectedPlant.positionY === y;
  };

  const handlePlantDrop = (x: number, y: number, plantId: number, plantName: string, imageBase64?: string | null) => {
    if (isEditMode) {
      try {
        // Check if this is a move operation (from within the garden)
        const dragDataStr = localStorage.getItem('dragData');
        if (dragDataStr) {
          const dropData = JSON.parse(dragDataStr);
          if (dropData.isMoveOperation) {
            // This is a move operation
            dispatch(moveTempPlant({
              fromX: dropData.fromX,
              fromY: dropData.fromY,
              toX: x,
              toY: y
            }));
            
            // Clear drag data after handling the move
            localStorage.removeItem('dragData');
          } else {
            // This is a new plant being added
            onPlantDropped(x, y, plantId);
          }
        } else {
          // No drag data available, treat as new plant
          onPlantDropped(x, y, plantId);
        }
      } catch (error) {
        console.error('Error handling plant drop:', error);
        // Fallback to adding a new plant
        onPlantDropped(x, y, plantId);
      }
    }
  };

  // Create grid cells based on garden size
  const renderGrid = () => {
    const cells: JSX.Element[] = [];
    const displayXSize = isEditMode && tempGarden ? tempGarden.xSize : garden.xSize;
    const displayYSize = isEditMode && tempGarden ? tempGarden.ySize : garden.ySize;
    
    for (let y = 0; y < displayYSize; y++) {
      for (let x = 0; x < displayXSize; x++) {
        const plant = findPlantAtPosition(x, y);
        cells.push(
          <GardenCell
            key={`cell-${x}-${y}`}
            x={x}
            y={y}
            plant={plant}
            onDropPlant={handlePlantDrop}
            isEditMode={isEditMode}
            isTempPlant={isEditMode && !!tempGarden}
            onPlantClick={onPlantClick}
            isSelected={isPlantSelected(x, y)} // Pass whether this cell is selected
          />
        );
      }
    }
    return cells;
  };

  const handleXSizeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newSize = parseInt(e.target.value) || 1;
    if (newSize > 0 && newSize <= 10) {
      onSizeChange(newSize, tempGarden?.ySize || garden.ySize);
    }
  };

  const handleYSizeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newSize = parseInt(e.target.value) || 1;
    if (newSize > 0 && newSize <= 10) {
      onSizeChange(tempGarden?.xSize || garden.xSize, newSize);
    }
  };

  return (
    <div className="garden-grid-container">
      <div 
        className="garden-grid"
        style={{ 
          gridTemplateColumns: `repeat(${isEditMode && tempGarden ? tempGarden.xSize : garden.xSize}, 100px)`,
          gridTemplateRows: `repeat(${isEditMode && tempGarden ? tempGarden.ySize : garden.ySize}, 100px)`
        }}
      >
        {renderGrid()}
      </div>
      
      {isEditMode && (
        <div className="garden-size-controls">
          {plantsAtRisk > 0 && (
            <div className="plants-at-risk-warning">
              <i className="fas fa-exclamation-triangle"></i>
              Warning: {plantsAtRisk} plant{plantsAtRisk === 1 ? '' : 's'} will be removed with this size change
            </div>
          )}
          
          <div className="garden-size-editor">
            <label>
              Width:
              <input
                type="number"
                min="1"
                max="10"
                value={tempGarden ? tempGarden.xSize : garden.xSize}
                onChange={handleXSizeChange}
                className="form-control"
              />
            </label>
            <label>
              Height:
              <input
                type="number"
                min="1"
                max="10"
                value={tempGarden ? tempGarden.ySize : garden.ySize}
                onChange={handleYSizeChange}
                className="form-control"
              />
            </label>
          </div>
        </div>
      )}
    </div>
  );
};

export default GardenGrid;