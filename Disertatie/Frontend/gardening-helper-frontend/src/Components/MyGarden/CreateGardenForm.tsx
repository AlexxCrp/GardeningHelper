import axios from 'axios';
import React, { useState } from 'react';
import { createGarden } from '../../Redux/Garden/GardenThunk';
import { useAppDispatch } from '../../Redux/Hooks/ReduxHooks';

// You'll need to replace 'YOUR_OPENCAGE_API_KEY' with your actual key
const OPENCAGE_API_KEY = 'c4dbbe8c2141463589a5ae7e948dc509';
const OPENCAGE_REVERSE_URL = 'https://api.opencagedata.com/geocode/v1/json'; // Use json endpoint for reverse geocoding

interface CreateGardenFormProps {
  onCancel: () => void;
}

// LocationSuggestion interface is not strictly needed anymore as we don't process suggestions directly,
// but keeping it as it describes the reverse geocoding result structure components.
interface LocationSuggestion {
  formatted: string; // The display text for the suggestion
  components: { // Detailed address components
    city?: string;
    town?: string;
    village?: string;
    country?: string;
    country_code?: string;
    // ... other components
  };
  geometry: { // Latitude and Longitude
    lat: number;
    lng: number;
  };
}


const CreateGardenForm: React.FC<CreateGardenFormProps> = ({ onCancel }) => {
  const dispatch = useAppDispatch();
  const [xSize, setXSize] = useState<number>(3);
  const [ySize, setYSize] = useState<number>(3);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // Location states (only for GPS)
  // Manual location states and locationType state removed
  const [country, setCountry] = useState<string>(''); // Kept to store reverse geocoding result
  const [city, setCity] = useState<string>('');    // Kept to store reverse geocoding result
  const [latitude, setLatitude] = useState<number | null>(null);
  const [longitude, setLongitude] = useState<number | null>(null);
  const [isGettingLocation, setIsGettingLocation] = useState<boolean>(false);
  const [locationError, setLocationError] = useState<string | null>(null);

  // Autocomplete states and refs removed

  // --- Autocomplete / Location API Logic ---

  // Debounced function to fetch suggestions - REMOVED as manual input is removed
  // const fetchSuggestions = useCallback(...);

  // Input change handlers for Country and City - REMOVED

  // Handle selecting a country/city suggestion - REMOVED

  // Close suggestions when clicking outside - REMOVED useEffect listener

  // --- GPS Location Logic ---

  // Use OpenCage Reverse Geocoding instead of Nominatim for consistency
  const getCityAndCountryFromCoords = async (lat: number, lng: number) => {
      try {
          // Using OpenCage Reverse Geocoding API
          const response = await axios.get(OPENCAGE_REVERSE_URL, {
              params: {
                  q: `${lat},${lng}`,
                  key: OPENCAGE_API_KEY
              }
          });

          if (response.data.results && response.data.results.length > 0) {
              const components = response.data.results[0].components;
              setCity(components.city || components.town || components.village || '');
              setCountry(components.country || '');
          } else {
              setLocationError('Could not determine city/country from location.');
              setCity(''); // Clear city/country if reverse geocoding fails
              setCountry('');
          }
      } catch (err) {
          console.error("Error during reverse geocoding:", err);
          setLocationError('Failed to get location details.');
          setCity(''); // Clear city/country on error
          setCountry('');
      } finally {
          // Only set setIsGettingLocation(false) after both GPS and reverse geocoding are done
          // or if reverse geocoding failed.
          // Let's keep it true until both are settled.
      }
  };


  const handleGetCurrentLocation = () => {
    if (!navigator.geolocation) {
      setLocationError('Geolocation is not supported by your browser');
      return;
    }

    setIsGettingLocation(true);
    setLocationError(null);
    // Clear previous location data when trying GPS
    setCity('');
    setCountry('');
    setLatitude(null);
    setLongitude(null);


    navigator.geolocation.getCurrentPosition(
      (position) => {
        const lat = position.coords.latitude;
        const lng = position.coords.longitude;
        setLatitude(lat);
        setLongitude(lng);

        // Use OpenCage Reverse Geocoding to get city and country
        getCityAndCountryFromCoords(lat, lng).finally(() => {
            // Set isGettingLocation to false after reverse geocoding completes (success or fail)
            setIsGettingLocation(false);
        });

      },
      (error) => {
        let errorMessage = 'Failed to get your location';
        switch (error.code) {
          case error.PERMISSION_DENIED:
            errorMessage = 'Location permission denied. Please allow location access in your browser settings.';
            break;
          case error.POSITION_UNAVAILABLE:
            errorMessage = 'Location information is unavailable.';
            break;
          case error.TIMEOUT:
            errorMessage = 'Location request timed out.';
            break;
        }
        setLocationError(errorMessage);
        setIsGettingLocation(false); // Ensure loading is turned off on error
        setLatitude(null); // Clear lat/lng on error
        setLongitude(null);
        setCity(''); // Clear city/country on error
        setCountry('');
      },
      { enableHighAccuracy: false, timeout: 15000, maximumAge: 0 } // Increased timeout slightly
    );
  };


  // --- Form Submission Logic ---

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (xSize <= 0 || ySize <= 0) {
      setError('Garden dimensions must be positive numbers');
      return;
    }

    if (xSize > 10 || ySize > 10) {
      setError('Maximum garden size is 10x10');
      return;
    }

    // Location validation (only for GPS)
    if (latitude === null || longitude === null) {
      setError('Please get your current location.'); // Updated error message
      return;
    }
    // City and Country should ideally be populated by reverse geocoding when using GPS.
    // We can add a check here, but the reverse geocoding is async.
    // For simplicity now, we'll send whatever city/country results we got from reverse geocoding
    // and the required lat/lng.


    setIsSubmitting(true);
    setError(null);

    try {
      // Send the location data based on the current state (populated by GPS/reverse geocoding)
      const gardenDataToSend = {
          xSize,
          ySize,
          city: city || 'Unknown City', // Send city from state (gps result)
          country: country || 'Unknown Country', // Send country from state (gps result)
          latitude: latitude !== null ? latitude : undefined, // Send lat/lng (should not be null if validation passes)
          longitude: longitude !== null ? longitude : undefined, // Send lat/lng (should not be null if validation passes)
      };

      await dispatch(createGarden(gardenDataToSend)).unwrap();
      onCancel(); // Close form on success
    } catch (err: any) { // Type error as any for simpler handling
      setError(err.message || 'Failed to create garden');
    } finally {
      setIsSubmitting(false);
    }
  };


  return (
    <div className="auth-card">
      <div className="auth-header">
        <h3>Create Your Garden</h3>
      </div>
      <div className="auth-body">
        {error && (
          <div className="alert alert-danger mb-3">
            {error}
          </div>
        )}
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="xSize" className="form-label">Width (meters)</label>
            <input
              type="number"
              className="form-control auth-input"
              id="xSize"
              value={xSize}
              onChange={(e) => setXSize(parseInt(e.target.value))}
              min="1"
              max="10"
              required
            />
          </div>
          <div className="mb-3">
            <label htmlFor="ySize" className="form-label">Length (meters)</label>
            <input
              type="number"
              className="form-control auth-input"
              id="ySize"
              value={ySize}
              onChange={(e) => setYSize(parseInt(e.target.value))}
              min="1"
              max="10"
              required
            />
          </div>

          <div className="mb-4">
            <h5>Garden Location</h5>
            {/* Location type radio buttons removed */}

            {/* Manual location input fields removed */}

            {/* GPS Location Button and Display */}
            <div>
              <button
                type="button"
                className="btn btn-secondary mb-2"
                onClick={handleGetCurrentLocation}
                disabled={isGettingLocation}
              >
                {isGettingLocation ? 'Getting Location...' : 'Get Current Location'}
              </button>

              {locationError && (
                <div className="alert alert-danger mb-2 small">
                  {locationError}
                </div>
              )}

              {/* Display acquired location details */}
              {latitude !== null && longitude !== null && (
                <div className="mb-3">
                  <div className="small text-success">
                     <i className="bi bi-geo-alt-fill me-1"></i>
                      {/* Display city/country if reverse geocoding was successful */}
                      Location acquired: {city && country ? `${city}, ${country}` : `Lat: ${latitude.toFixed(4)}, Lng: ${longitude.toFixed(4)}`}
                      {/* Add a spinner or message if reverse geocoding is still in progress (optional, handled by isGettingLocation) */}
                      {/* {isGettingLocation && !city && !country && " (Getting details...)"} */}
                  </div>
                </div>
              )}
            </div>
          </div>

          <div className="d-flex justify-content-between mt-4">
            <button
              type="button"
              className="btn btn-secondary"
              onClick={onCancel}
              disabled={isSubmitting}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn btn-success"
              // Submit button disabled if submitting, getting location, or lat/lng are not yet acquired
              disabled={isSubmitting || isGettingLocation || latitude === null || longitude === null}
            >
              {isSubmitting ? 'Creating...' : 'Create Garden'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateGardenForm;