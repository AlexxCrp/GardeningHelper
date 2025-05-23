import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../Redux/Hooks/ReduxHooks';
import { selectPlantById, selectPlantDetailsById, selectPlantsError, selectPlantsLoading } from '../../Redux/Plant/PlantSelector';
import { fetchPlant, fetchPlantDetails } from '../../Redux/Plant/PlantThunk';
import './PlantDetails.css';

const PlantDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const plantId = Number(id);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const plantDetails = useAppSelector(selectPlantDetailsById(plantId));
  const plant = useAppSelector(selectPlantById(plantId));
  const isLoading = useAppSelector(selectPlantsLoading);
  const error = useAppSelector(selectPlantsError);
  const [activeImageIndex, setActiveImageIndex] = useState(0);
  const location = useLocation();

  useEffect(() => {
    if (plantId) {
      dispatch(fetchPlant(plantId));
      dispatch(fetchPlantDetails(plantId));
    }
  }, [dispatch, plantId]);

  const handleGoBack = () => {
    // Check where we came from based on navigation state
    const from = location.state?.from;
    
    if (from === 'Garden') {
      navigate('/');
    } else {
      // Default to /plants if no state or from PlantCardContainer
      navigate('/plants');
    }
  };

  const handleImageChange = (index: number) => {
    setActiveImageIndex(index);
  };

  if (isLoading) return <div className="loading-container"><p className="loading-text">Loading plant details...</p></div>;
  if (error) return <div className="error-container"><p className="error-text">{error}</p></div>;
  if (!plantDetails) return <div className="error-container"><p className="error-text">Plant details not found</p></div>;

  // Define all enum mappings with proper TypeScript index signatures
  const bloomSeasonEnum: { [key: number]: string } = {
    0: 'None', 
    1: 'Spring', 
    2: 'Summer', 
    3: 'Fall', 
    4: 'Winter',
    5: 'Spring-Summer',
    6: 'Summer-Fall',
    7: 'Fall-Winter',
    8: 'Winter-Spring',
    9: 'Year Round'
  };
  
  const lifecycleEnum: { [key: number]: string } = {
    0: 'Annual',
    1: 'Biennial',
    2: 'Perennial',
    3: 'Evergreen'
  };
  
  const waterNeedsEnum: { [key: number]: string } = {
    0: 'Low',
    1: 'Medium',
    2: 'High'
  };
  
  const difficultyLevelEnum: { [key: number]: string } = {
    0: 'Beginner',
    1: 'Intermediate',
    2: 'Advanced'
  };
  
  const harvestTimeEnum: { [key: number]: string } = {
    0: 'Spring',
    1: 'Summer',
    2: 'Fall',
    3: 'Winter'
  };
  
  const purposeEnum: { [key: number]: string } = {
    0: 'Ornamental',
    1: 'Edible',
    2: 'Medicinal',
    3: 'Aromatic',
    4: 'Companion'
  };

  // Format specific enum types directly with proper TypeScript handling
  const formatBloomSeason = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return 'Unknown';
    return bloomSeasonEnum[value] !== undefined ? bloomSeasonEnum[value] : `Value ${value}`;
  };

  const formatLifecycle = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return 'Unknown';
    return lifecycleEnum[value] !== undefined ? lifecycleEnum[value] : `Value ${value}`;
  };

  const formatWaterNeeds = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return 'Unknown';
    return waterNeedsEnum[value] !== undefined ? waterNeedsEnum[value] : `Value ${value}`;
  };

  const formatDifficultyLevel = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return 'Unknown';
    return difficultyLevelEnum[value] !== undefined ? difficultyLevelEnum[value] : `Value ${value}`;
  };

  const formatHarvestTime = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return 'Unknown';
    return harvestTimeEnum[value] !== undefined ? harvestTimeEnum[value] : `Value ${value}`;
  };

  const formatPurpose = (value: number | null | undefined): string => {
    if (value === null || value === undefined) return 'Unknown';
    return purposeEnum[value] !== undefined ? purposeEnum[value] : `Value ${value}`;
  };

  // Generic formatter for string values
  const formatStringValue = (value: string | null | undefined) => {
    if (value === null || value === undefined) return 'Unknown';
    return String(value)
      .replace(/([A-Z])/g, ' $1') // Insert space before capital letters
      .replace(/^./, (str) => str.toUpperCase()); // Capitalize first letter
  };

  const renderArrayAsList = (items: string[] | undefined) => {
    if (!items || items.length === 0) return 'None';
    
    return (
      <ul className="detail-list">
        {items.map((item, index) => (
          <li key={index}>{item}</li>
        ))}
      </ul>
    );
  };

  return (
    <div className="plant-details-container">
      <button onClick={handleGoBack} className="back-button">
        <span className="back-icon">←</span> Back
      </button>
      
      <div className="plant-details-header">
        <div className="plant-gallery">
          <div className="main-image-container">
            {plantDetails.imageUrls && plantDetails.imageUrls.length > 0 ? (
              <img 
                src={plantDetails.imageUrls[activeImageIndex]} 
                alt={`${plant?.name || 'Plant'}`} 
                className="main-image"
                onError={(e) => {
                  console.error('Image failed to load');
                  (e.target as HTMLImageElement).src = 'https://via.placeholder.com/400x300?text=Image+Not+Available';
                }}
              />
            ) : (
              <div className="no-image">No image available</div>
            )}
          </div>
          
          {plantDetails.imageUrls && plantDetails.imageUrls.length > 1 && (
            <div className="thumbnail-container">
              {plantDetails.imageUrls.map((url, index) => (
                <div 
                  key={index} 
                  className={`thumbnail ${index === activeImageIndex ? 'active' : ''}`}
                  onClick={() => handleImageChange(index)}
                >
                  <img 
                    src={url} 
                    alt={`Thumbnail ${index + 1}`}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = 'https://via.placeholder.com/100x100?text=NA';
                    }}
                  />
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="plant-summary">
          <h1 className="plant-title">{plant?.name || 'Plant Details'}</h1>
          
          <div className="quick-stats">
            <div className="stat">
              <span className="stat-label">Bloom Season</span>
              <span className="stat-value">{formatBloomSeason(plantDetails.bloomSeason)}</span>
            </div>
            <div className="stat">
              <span className="stat-label">Lifecycle</span>
              <span className="stat-value">{formatLifecycle(plantDetails.lifecycle)}</span>
            </div>
            <div className="stat">
              <span className="stat-label">Water Needs</span>
              <span className="stat-value">{formatWaterNeeds(plantDetails.waterNeeds)}</span>
            </div>
            <div className="stat">
              <span className="stat-label">Difficulty</span>
              <span className="stat-value">{formatDifficultyLevel(plantDetails.difficultyLevel)}</span>
            </div>
          </div>
        </div>
      </div>

      <div className="plant-details-main">
        <div className="details-section">
          <h2>Plant Characteristics</h2>
          <div className="details-grid">
            <div className="detail-item">
              <h3>Native To</h3>
              <p>{plantDetails.nativeTo || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>pH Level</h3>
              <p>{plantDetails.idealPhLevel || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Growing Zones</h3>
              <p>{plantDetails.growingZones || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Height at Maturity</h3>
              <p>{plantDetails.heightAtMaturity || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Spread at Maturity</h3>
              <p>{plantDetails.spreadAtMaturity || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Days to Germination</h3>
              <p>{plantDetails.daysToGermination || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Days to Maturity</h3>
              <p>{plantDetails.daysToMaturity || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Planting Depth</h3>
              <p>{plantDetails.plantingDepth || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Spacing Between Plants</h3>
              <p>{plantDetails.spacingBetweenPlants || 'Unknown'}</p>
            </div>
            <div className="detail-item">
              <h3>Purposes</h3>
              <p>
                {plantDetails.purposes && Array.isArray(plantDetails.purposes) && plantDetails.purposes.length > 0 
                  ? plantDetails.purposes.map(purpose => formatPurpose(purpose)).join(', ') 
                  : 'Unknown'}
              </p>
            </div>
          </div>
        </div>

        <div className="details-section">
          <h2>Growing Instructions</h2>
          <div className="details-grid">
            <div className="detail-item">
              <h3>Propagation Methods</h3>
              <p>{plantDetails.propagationMethods || 'Unknown'}</p>
            </div>
            <div className="detail-item full-width">
              <h3>Pruning Instructions</h3>
              <p>{plantDetails.pruningInstructions || 'No specific instructions available'}</p>
            </div>
            <div className="detail-item full-width">
              <h3>Pest Management</h3>
              <p>{plantDetails.pestManagement || 'No specific instructions available'}</p>
            </div>
            <div className="detail-item full-width">
              <h3>Disease Management</h3>
              <p>{plantDetails.diseaseManagement || 'No specific instructions available'}</p>
            </div>
            <div className="detail-item">
              <h3>Fertilization Schedule</h3>
              <p>{plantDetails.fertilizationSchedule || 'No specific schedule available'}</p>
            </div>
            <div className="detail-item">
              <h3>Winter Care</h3>
              <p>{plantDetails.winterCare || 'No specific care instructions available'}</p>
            </div>
          </div>
        </div>

        <div className="details-section">
          <h2>Harvesting & Uses</h2>
          <div className="details-grid">
            <div className="detail-item">
              <h3>Harvesting Tips</h3>
              <p>{plantDetails.harvestingTips || 'No specific tips available'}</p>
            </div>
            <div className="detail-item">
              <h3>Storage Tips</h3>
              <p>{plantDetails.storageTips || 'No specific storage information available'}</p>
            </div>
            <div className="detail-item">
              <h3>Culinary Uses</h3>
              <p>{plantDetails.culinaryUses || 'No culinary uses listed'}</p>
            </div>
            <div className="detail-item">
              <h3>Medicinal Uses</h3>
              <p>{plantDetails.medicinalUses || 'No medicinal uses listed'}</p>
            </div>
          </div>
        </div>

        {(plantDetails.historicalNotes || plantDetails.additionalNotes) && (
          <div className="details-section">
            <h2>Additional Information</h2>
            {plantDetails.historicalNotes && (
              <div className="detail-item full-width">
                <h3>Historical Notes</h3>
                <p>{plantDetails.historicalNotes}</p>
              </div>
            )}
            {plantDetails.additionalNotes && (
              <div className="detail-item full-width mt-15">
                <h3>Additional Notes</h3>
                <p>{plantDetails.additionalNotes}</p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default PlantDetails;