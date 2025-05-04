import { createAsyncThunk } from '@reduxjs/toolkit';
import { AddPlantToGardenRequestDTO } from '../../Models/API/DTOs/Auto/Request/addPlantToGardenRequestDTO';
import { CreateGardenRequestDTO } from '../../Models/API/DTOs/Auto/Request/createGardenRequestDTO';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { UserGardenResponseDTO } from '../../Models/API/DTOs/Auto/Response/userGardenResponseDTO';
import { GardenService } from '../../Services/GardenService';
import { RootState } from '../Store';

export const fetchUserGarden = createAsyncThunk<UserGardenResponseDTO | null, string, { rejectValue: string }>(
  'garden/fetchUserGarden',
  async (id, { rejectWithValue }) => {
    try {
      return await GardenService.getUserGarden();
    } catch (error: any) {
      if (error instanceof Error && error.message.includes('404')) {
        return null;
      }
      return rejectWithValue(error.message || 'Failed to fetch garden');
    }
  }
);

export const createGarden = createAsyncThunk<UserGardenResponseDTO, CreateGardenRequestDTO, { rejectValue: string }>(
  'garden/createGarden',
  async (gardenData, { rejectWithValue }) => {
    try {
      return await GardenService.createGarden(gardenData);
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to create garden');
    }
  }
);

export const updateGarden = createAsyncThunk<UserGardenResponseDTO, CreateGardenRequestDTO, { rejectValue: string }>(
  'garden/updateGarden',
  async (gardenData, { rejectWithValue }) => {
    try {
      return await GardenService.updateGarden(gardenData);
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update garden');
    }
  }
);

export const addPlantToGarden = createAsyncThunk<GardenPlantResponseDTO, AddPlantToGardenRequestDTO, { rejectValue: string }>(
  'garden/addPlantToGarden',
  async (plantData, { rejectWithValue }) => {
    try {
      return await GardenService.addPlantToGarden(plantData);
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to add plant');
    }
  }
);

export const removePlantFromGarden = createAsyncThunk<number, number, { rejectValue: string }>(
  'garden/removePlantFromGarden',
  async (plantId, { rejectWithValue }) => {
    try {
      await GardenService.removePlantFromGarden(plantId);
      return plantId;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to remove plant');
    }
  }
);

// New thunk for batch updating the garden
export const batchUpdateGarden = createAsyncThunk<UserGardenResponseDTO, void, { 
  rejectValue: string, 
  state: RootState 
}>(
  'garden/batchUpdateGarden',
  async (_, { getState, rejectWithValue, dispatch }) => {
    try {
      const state = getState();
      const { garden, tempGarden } = state.garden;
      
      if (!garden || !tempGarden) {
        return rejectWithValue('No garden or changes to save');
      }
      
      // First, update garden size if needed
      if (garden.xSize !== tempGarden.xSize || garden.ySize !== tempGarden.ySize) {
        await dispatch(updateGarden({ 
          xSize: tempGarden.xSize, 
          ySize: tempGarden.ySize 
        })).unwrap();
      }
      
      // Process each plant change in sequence
      for (const plant of tempGarden.plants) {
        switch (plant.operation) {
          case 'add':
            // Only add if this is a new plant (no ID)
            if (!plant.id) {
              await dispatch(addPlantToGarden({
                plantId: plant.plantId,
                positionX: plant.positionX,
                positionY: plant.positionY
              })).unwrap();
            }
            break;
            
          case 'move':
            // For moves, we need to remove the plant first then add it at the new position
            if (plant.id) {
              await dispatch(removePlantFromGarden(plant.id)).unwrap();
              await dispatch(addPlantToGarden({
                plantId: plant.plantId,
                positionX: plant.positionX,
                positionY: plant.positionY
              })).unwrap();
            }
            break;
            
          case 'remove':
            if (plant.id) {
              await dispatch(removePlantFromGarden(plant.id)).unwrap();
            }
            break;
        }
      }
      
      // Fetch the final state of the garden
      return await GardenService.getUserGarden();
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update garden');
    }
  }
);