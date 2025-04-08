import { PlantDTO } from '../../Models/API/DTOs/Auto/Response/plantDTO';
import { createApiClient } from '../ApiClient';

// Create a plant-specific API client
const PlantApiClient = createApiClient<PlantDTO>('Plant');

export default PlantApiClient;