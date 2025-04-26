import { GardenPlantResponseDTO } from "../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO";
import { UserGardenResponseDTO } from "../../Models/API/DTOs/Auto/Response/userGardenResponseDTO";

export interface GardenState {
    garden: UserGardenResponseDTO | null;
    availablePlants: GardenPlantResponseDTO[];
    loading: boolean;
    error: string | null;
    isCreatingGarden: boolean;
  }
  
  export const GARDEN_INITIAL_STATE: GardenState = {
    garden: null,
    availablePlants: [],
    loading: false,
    error: null,
    isCreatingGarden: false
  };