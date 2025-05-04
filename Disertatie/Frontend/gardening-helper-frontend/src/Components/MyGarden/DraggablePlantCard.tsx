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
    
    // For in-garden cells, use the default drag behavior (ghost image)
    if (inGardenCell) {
      const plantData = {
        plantId: id,
        name,
        imageBase64,
        isMoveOperation: true
      };
      e.dataTransfer.setData('application/json', JSON.stringify(plantData));
      e.currentTarget.classList.add('dragging');
      return;
    }
  
    // For available plants, create a smaller drag image
    const dragImage = document.createElement('div');
    dragImage.style.width = '60px';
    dragImage.style.height = '60px';
    dragImage.style.background = '#3a3a3a';
    dragImage.style.borderRadius = '4px';
    dragImage.style.display = 'flex';
    dragImage.style.justifyContent = 'center';
    dragImage.style.alignItems = 'center';
    dragImage.style.padding = '4px';
    
    if (imageBase64) {
      const img = document.createElement('img');
      img.src = imageBase64;
      img.style.maxWidth = '100%';
      img.style.maxHeight = '100%';
      img.style.objectFit = 'contain';
      dragImage.appendChild(img);
    } else {
      dragImage.textContent = name.substring(0, 10);
      dragImage.style.fontSize = '10px';
      dragImage.style.color = '#fff';
      dragImage.style.textAlign = 'center';
    }
  
    document.body.appendChild(dragImage);
    e.dataTransfer.setDragImage(dragImage, 30, 30);
    
    // Remove the drag image after a short delay
    setTimeout(() => document.body.removeChild(dragImage), 0);
  
    const plantData = {
      plantId: id,
      name,
      imageBase64
    };
    
    e.dataTransfer.setData('application/json', JSON.stringify(plantData));
    e.currentTarget.classList.add('dragging');
    e.currentTarget.style.opacity = '0.4';
  };
  
  const handleDragEnd = (e: React.DragEvent<HTMLDivElement>) => {
    e.currentTarget.classList.remove('dragging');
    e.currentTarget.style.opacity = '1';
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