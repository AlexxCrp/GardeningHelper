// src/Components/PlantCard/DraggablePlantCard.tsx
import React from 'react';
import './PlantCard.css';

interface DraggablePlantCardProps {
  id: number;
  name: string;
  imageBase64?: string | null;
  isDraggable?: boolean;
}

const DraggablePlantCard: React.FC<DraggablePlantCardProps> = ({ 
  id, 
  name, 
  imageBase64, 
  isDraggable = true 
}) => {
  const handleDragStart = (e: React.DragEvent<HTMLDivElement>) => {
    if (!isDraggable) return;
    
    e.dataTransfer.setData('application/json', JSON.stringify({
      plantId: id,
      name
    }));
    
    e.currentTarget.classList.add('dragging');
  };

  const handleDragEnd = (e: React.DragEvent<HTMLDivElement>) => {
    e.currentTarget.classList.remove('dragging');
  };

  return (
    <div 
      className="plant-card"
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