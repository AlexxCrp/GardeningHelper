import React from 'react';
import './PlantCard.css';

interface DraggablePlantCardProps {
  id: number;
  name: string;
  imageBase64?: string | null;
  isDraggable?: boolean;
  inGardenCell?: boolean;
}

const DraggablePlantCard: React.FC<DraggablePlantCardProps> = ({ 
  id, 
  name, 
  imageBase64, 
  isDraggable = true,
  inGardenCell = false
}) => {
  const handleDragStart = (e: React.DragEvent<HTMLDivElement>) => {
    if (!isDraggable) return;
    
    // Store complete plant data in the drag event
    const plantData = {
      plantId: id,
      name,
      imageBase64
    };
    
    e.dataTransfer.setData('application/json', JSON.stringify(plantData));
    
    // Also store in localStorage as a backup for complex drag operations
    localStorage.setItem('dragData', JSON.stringify(plantData));
    
    e.currentTarget.classList.add('dragging');
  };

  const handleDragEnd = (e: React.DragEvent<HTMLDivElement>) => {
    e.currentTarget.classList.remove('dragging');
    // Clean up localStorage after drag operation completes
    localStorage.removeItem('dragData');
  };

  const cardClasses = [
    'plant-card',
    inGardenCell ? 'in-garden-cell' : ''
  ].filter(Boolean).join(' ');

  return (
    <div 
      className={cardClasses}
      draggable={isDraggable}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <div className="card-content">
        <div className="image-container">
        {imageBase64 ? (
          <img
            src={imageBase64}
            alt={name}
            className="plant-image"
            onError={(e) => {
              console.error('Image failed to load');
              (e.target as HTMLImageElement).style.display = 'none';
            }}
          />
        ) : (
          <div className="no-image">No image</div>
        )}
        </div>
        <div className="card-title">
          <h5 className="plant-name">{name}</h5>
        </div>
      </div>
    </div>
  );
};

export default DraggablePlantCard;