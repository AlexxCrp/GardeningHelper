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

export const batchUpdateGarden = createAsyncThunk<UserGardenResponseDTO, void, { 
  rejectValue: string, 
  state: RootState 
}>(
  'garden/batchUpdateGarden',
  async (_, { getState, rejectWithValue }) => {
    try {
      const state = getState();
      const { garden, tempGarden } = state.garden;
      
      if (!garden || !tempGarden) {
        return rejectWithValue('No garden or changes to save');
      }
      
      // Prepare all updates in a single batch
      const updates: Promise<any>[] = [];
      
      // Update garden size if needed
      if (garden.xSize !== tempGarden.xSize || garden.ySize !== tempGarden.ySize) {
        updates.push(GardenService.updateGarden({ 
          xSize: tempGarden.xSize, 
          ySize: tempGarden.ySize,
        }));
      }
      
      // Process plant changes
      for (const plant of tempGarden.plants) {
        switch (plant.operation) {
          case 'add':
            if (!plant.id) {
              updates.push(GardenService.addPlantToGarden({
                plantId: plant.plantId,
                positionX: plant.positionX,
                positionY: plant.positionY
              }));
            }
            break;
            
          case 'move':
            if (plant.id) {
              // Find the original plant in the garden to get its data
              const originalPlant = garden.gardenPlants.find(p => p.id === plant.id);
              if (originalPlant) {
                updates.push(
                  GardenService.updateGardenPlant({
                    gardenPlantId: plant.id,
                    positionX: plant.positionX,
                    positionY: plant.positionY,
                    lastWateredDate: originalPlant.lastWateredDate || new Date(),
                    lastSoilMoisture: originalPlant.lastSoilMoisture || 0
                  })
                );
              }
            }
            break;
            
          case 'remove':
            if (plant.id) {
              updates.push(GardenService.removePlantFromGarden(plant.id));
            }
            break;
        }
      }
      
      // Execute all updates in parallel
      await Promise.all(updates);
      
      // Return the updated garden state
      return await GardenService.getUserGarden();
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update garden');
    }
  }
);