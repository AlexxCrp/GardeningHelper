import { createAsyncThunk } from '@reduxjs/toolkit';
import { PlantCardDTO } from '../../Models/API/DTOs/Auto/Response/plantCardDTO';
import { PlantDTO } from '../../Models/API/DTOs/Auto/Response/plantDTO';
import { PlantDetailsResponseDTO } from '../../Models/API/DTOs/Auto/Response/plantDetailsResponseDTO';
import PlantApiClient from '../../Services/CustomApiClients/PlantApiClient';

// Fetch a single plant by ID
export const fetchPlant = createAsyncThunk<
  PlantDTO,
  number,
  { rejectValue: string }
>(
  'plant/fetchPlant',
  async (plantId, { rejectWithValue }) => {
    try {
      return await PlantApiClient.getById(plantId);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch plant');
    }
  }
);

// Fetch all plants
export const fetchAllPlants = createAsyncThunk<
  PlantDTO[],
  void,
  { rejectValue: string }
>(
  'plant/fetchAllPlants',
  async (_, { rejectWithValue }) => {
    try {
      return await PlantApiClient.getAll();
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch plants');
    }
  }
);

// Fetch plant cards
export const fetchCardPlants = createAsyncThunk<
  PlantCardDTO[],
  void,
  { rejectValue: string }
>(
  'plant/fetchPlantCards',
  async (_, { rejectWithValue }) => {
    try {
      return await PlantApiClient.customGet<PlantCardDTO[]>('/PlantCard');
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch plants');
    }
  }
);

// Fetch plant details by plant ID
export const fetchPlantDetails = createAsyncThunk<
  PlantDetailsResponseDTO,
  number,
  { rejectValue: string }
>(
  'plant/fetchPlantDetails',
  async (plantId, { rejectWithValue }) => {
    try {
      return await PlantApiClient.customGet<PlantDetailsResponseDTO>(`/Details?id=${plantId}`);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch plant details');
    }
  }
);

// Create a new plant
export const createPlant = createAsyncThunk<
  PlantDTO,
  Partial<PlantDTO>,
  { rejectValue: string }
>(
  'plant/createPlant',
  async (plantData, { rejectWithValue }) => {
    try {
      return await PlantApiClient.create(plantData);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to create plant');
    }
  }
);

// Update an existing plant
export const updatePlant = createAsyncThunk<
  PlantDTO,
  { id: number; data: Partial<PlantDTO> },
  { rejectValue: string }
>(
  'plant/updatePlant',
  async ({ id, data }, { rejectWithValue }) => {
    try {
      return await PlantApiClient.update(id, data);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to update plant');
    }
  }
);

// Delete a plant
export const deletePlant = createAsyncThunk<
  boolean,
  number,
  { rejectValue: string }
>(
  'plant/deletePlant',
  async (plantId, { rejectWithValue }) => {
    try {
      return await PlantApiClient.delete(plantId);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to delete plant');
    }
  }
);

// Custom thunk for plant-specific operations
export const waterPlant = createAsyncThunk<
  PlantDTO,
  number,
  { rejectValue: string }
>(
  'plant/waterPlant',
  async (plantId, { rejectWithValue }) => {
    try {
      return await PlantApiClient.customPost<{ plantId: number }, PlantDTO>('/water', { plantId });
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to water plant');
    }
  }
);

// Thunk for fetching plants by soil type
export const fetchPlantsBySoilType = createAsyncThunk<
  PlantDTO[],
  string,
  { rejectValue: string }
>(
  'plant/fetchPlantsBySoilType',
  async (soilType, { rejectWithValue }) => {
    try {
      return await PlantApiClient.customGet<PlantDTO[]>(`/soilType/${soilType}`);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch plants by soil type');
    }
  }
);