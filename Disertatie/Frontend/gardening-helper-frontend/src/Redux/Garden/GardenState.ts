import { GardenPlantResponseDTO } from "../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO";
import { UserGardenResponseDTO } from "../../Models/API/DTOs/Auto/Response/userGardenResponseDTO";

// Define a new interface for temporary plant changes
export interface TempGardenPlant {
  id?: number; // Only present for existing plants being moved
  plantId: number;
  plantName: string;
  positionX: number;
  positionY: number;
  base64Image?: string | null;
  originalPosition?: { x: number, y: number }; // Track original position for moves
  operation: 'add' | 'move' | 'remove'; // Track the operation type
}

export interface GardenState {
  garden: UserGardenResponseDTO | null;
  tempGarden: {
    plants: TempGardenPlant[];
    xSize: number;
    ySize: number;
  } | null;
  availablePlants: GardenPlantResponseDTO[];
  loading: boolean;
  error: string | null;
  isCreatingGarden: boolean;
}

export const GARDEN_INITIAL_STATE: GardenState = {
  garden: null,
  tempGarden: null,
  availablePlants: [],
  loading: false,
  error: null,
  isCreatingGarden: false
};