import React from 'react';
import { UserGardenResponseDTO } from '../../Models/API/DTOs/Auto/Response/userGardenResponseDTO';
import GardenCell from './GardenCell';

interface GardenGridProps {
  garden: UserGardenResponseDTO;
  onPlantDropped: (x: number, y: number, plantId: number) => void;
  isEditMode: boolean;
  tempXSize?: number;
  tempYSize?: number;
  setTempXSize?: (size: number) => void;
  setTempYSize?: (size: number) => void;
  plantsAtRisk: number;
}

const GardenGrid: React.FC<GardenGridProps> = ({ 
  garden, 
  isEditMode, 
  onPlantDropped,
  tempXSize,
  tempYSize,
  setTempXSize,
  setTempYSize,
  plantsAtRisk
}) => {
  const findPlantAtPosition = (x: number, y: number) => {
    return garden.gardenPlants.find(
      plant => plant.positionX === x && plant.positionY === y
    );
  };

  // Create grid cells based on garden size
  const renderGrid = () => {
    const cells = [];
    const displayXSize = isEditMode && tempXSize !== undefined ? tempXSize : garden.xSize;
    const displayYSize = isEditMode && tempYSize !== undefined ? tempYSize : garden.ySize;
    
    for (let y = 0; y < displayYSize; y++) {
      for (let x = 0; x < displayXSize; x++) {
        const plant = findPlantAtPosition(x, y);
        cells.push(
          <GardenCell
            key={`cell-${x}-${y}`}
            x={x}
            y={y}
            plant={plant}
            onDropPlant={onPlantDropped}
            isEditMode={isEditMode}
          />
        );
      }
    }
    return cells;
  };

  return (
    <div className="garden-grid-container">
      <div 
        className="garden-grid"
        style={{ 
          gridTemplateColumns: `repeat(${isEditMode && tempXSize !== undefined ? tempXSize : garden.xSize}, 100px)`,
          gridTemplateRows: `repeat(${isEditMode && tempYSize !== undefined ? tempYSize : garden.ySize}, 100px)`
        }}
      >
        {renderGrid()}
      </div>
      
      {isEditMode && setTempXSize && setTempYSize && (
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
                value={tempXSize !== undefined ? tempXSize : garden.xSize}
                onChange={(e) => setTempXSize(parseInt(e.target.value) || 1)}
                className="form-control"
              />
            </label>
            <label>
              Height:
              <input
                type="number"
                min="1"
                value={tempYSize !== undefined ? tempYSize : garden.ySize}
                onChange={(e) => setTempYSize(parseInt(e.target.value) || 1)}
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