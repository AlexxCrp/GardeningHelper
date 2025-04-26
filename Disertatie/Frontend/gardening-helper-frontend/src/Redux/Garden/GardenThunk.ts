import { createAsyncThunk } from '@reduxjs/toolkit';
import { AddPlantToGardenRequestDTO } from '../../Models/API/DTOs/Auto/Request/addPlantToGardenRequestDTO';
import { CreateGardenRequestDTO } from '../../Models/API/DTOs/Auto/Request/createGardenRequestDTO';
import { GardenPlantResponseDTO } from '../../Models/API/DTOs/Auto/Response/gardenPlantResponseDTO';
import { UserGardenResponseDTO } from '../../Models/API/DTOs/Auto/Response/userGardenResponseDTO';
import { GardenService } from '../../Services/GardenService';

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
      return rejectWithValue(error.message || 'Failed to create garden');
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
