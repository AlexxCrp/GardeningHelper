import { useNavigate } from 'react-router-dom';
import './PlantCard.css';

interface PlantCardProps {
  id?: number;
  name: string;
  imageBase64: string | null;
}

const PlantCard: React.FC<PlantCardProps> = ({ id, name, imageBase64 }) => {
  const navigate = useNavigate();

  const handleCardClick = () => {
    if (id) {
      navigate(`/plants/${id}`);
    }
  };

  return (
    <div className="plant-card" onClick={handleCardClick} style={{ cursor: id ? 'pointer' : 'default' }}>
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

export default PlantCard;